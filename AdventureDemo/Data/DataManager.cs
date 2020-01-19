using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using Newtonsoft.Json;

namespace AdventureDemo
{
    class DataManager
    {
        #region Singleton
        public static DataManager _instance;
        public static DataManager instance {
            get {
                if( _instance == null ) {
                    _instance = new DataManager();
                }

                return _instance;
            }
        }
        #endregion

        #region Fields

        string root;

        DirectoryInfo dataDirectory;

        Settings settings;
        FileInfo settingsFile;

        public struct DataPointer
        {
            public string filePath;
            public int start;
            public int length;

            public DataPointer( string p, int s, int e )
            {
                filePath = p;
                start = s;
                length = e;
            }
        }
        Dictionary<string, DataPointer> scenarioFiles;
        Dictionary<string, DataPointer> objectFiles;
        Dictionary<string, DataPointer> materialFiles;
        Dictionary<string, DataPointer> spawnListsFiles;

        // XXX: Implement hybrid memory/file loading system where recently loaded data objects are stored
        //      in memory using a queue based first-used-first-out system

        #endregion

        #region Initialization Methods

        private DataManager()
        {
            root = $@"{Directory.GetCurrentDirectory()}\..\";

            scenarioFiles = new Dictionary<string, DataPointer>();
            objectFiles = new Dictionary<string, DataPointer>();
            materialFiles = new Dictionary<string, DataPointer>();
            spawnListsFiles = new Dictionary<string, DataPointer>();
        }

        public void Init( AdventureApp app )
        {
            LoadSettings();

            SetupData();
        }

        void LoadSettings()
        {
            settingsFile = new FileInfo(root + $@"settings.config");
            if( !settingsFile.Exists ) {
                settings = new Settings();

                string jsonSettings = JsonConvert.SerializeObject(settings, Formatting.Indented);
                StreamWriter writer = new StreamWriter(settingsFile.FullName);
                writer.Write(jsonSettings);
                writer.Close();
            } else {
                StreamReader reader = new StreamReader(settingsFile.FullName);

                string jsonSettings = reader.ReadToEnd();
                settings = JsonConvert.DeserializeObject<Settings>(jsonSettings);
            }
        }
        public Settings GetSettings()
        {
            return new Settings(settings);
        }

        #endregion

        #region File Loading Methods

        public struct ParseData<T>
        {
            public T data;
            public int start;
            public int length;

            public ParseData(T d, int s, int e)
            {
                data = d;
                start = s;
                length = e;
            }
        }

        void SetupData()
        {
            dataDirectory = new DirectoryInfo(root + "Data");
            if( !dataDirectory.Exists ) {
                dataDirectory.Create();
            }

            foreach( DirectoryInfo dir in dataDirectory.GetDirectories() ) {
                LoadDirectory(dir);
            }
        }
        public void LoadDirectory( string subPath )
        {
            DirectoryInfo directory = new DirectoryInfo(dataDirectory.FullName + "\\" + subPath);
            if( !directory.Exists ) { directory.Create(); return; }

            LoadDirectory(directory);
        }
        public void LoadDirectory( DirectoryInfo directory )
        {
            FileInfo[] files = directory.GetFiles();
            foreach( FileInfo file in files ) {
                switch( file.Extension ) {
                    case ".scenario":
                        AddFile<ScenarioData>(file, scenarioFiles);
                        break;
                    case ".object":
                        AddFile<ObjectData>(file, objectFiles);
                        break;
                    case ".material":
                        AddFile<ObjectData>(file, materialFiles);
                        break;
                    case ".spawn":
                        AddFile<SpawnData>(file, spawnListsFiles);
                        break;
                }
            }

            foreach( DirectoryInfo dir in directory.GetDirectories() ) {
                LoadDirectory(dir);
            }
        }
        void AddFile<T>( FileInfo file, Dictionary<string, DataPointer> dict )
            where T : BasicData
        {
            List<ParseData<T>> datas = ParseFile<T>(file);

            foreach( ParseData<T> data in datas ) {
                dict.Add( data.data.id, new DataPointer(file.FullName, data.start, data.length) );
            }
        }

        List<ParseData<T>> ParseFile<T>( string path )
        {
            return ParseFile<T>( new FileInfo(path));
        }
        List<ParseData<T>> ParseFile<T>( FileInfo file )
        {
            if( !file.Exists ) { return new List<ParseData<T>>(); }

            StreamReader reader = file.OpenText();
            string contents = reader.ReadToEnd();

            List<ParseData<T>> datas = new List<ParseData<T>>();
            int startIndex = 0;
            int endIndex;
            do {
                endIndex = contents.IndexOf("};", startIndex);
                if( endIndex == -1 ) { break; }

                endIndex++;
                string json = contents.Substring(startIndex, endIndex - startIndex);
                datas.Add(new ParseData<T>(
                    JsonConvert.DeserializeObject<T>(json),
                    startIndex, endIndex - startIndex
                ) );
                startIndex = endIndex + 1;
            } while( endIndex != -1 );

            return datas;
        }

        #endregion

        #region Data Methods

        public ScenarioData[] GetScenarioDatas()
        {
            List<ScenarioData> datas = new List<ScenarioData>();

            List<string> files = new List<string>();
            foreach( DataPointer pointer in scenarioFiles.Values ) {
                if( !files.Contains(pointer.filePath) ) {
                    files.Add(pointer.filePath);

                    List<ParseData<ScenarioData>> parseDatas = ParseFile<ScenarioData>(pointer.filePath);
                    foreach( ParseData<ScenarioData> parseData in parseDatas ) {
                        datas.Add(parseData.data);
                    }
                }
            }

            return datas.ToArray();
        }
        public ScenarioData GetScenarioData( string id )
        {
            return GetData<ScenarioData>(id, scenarioFiles);
        }
        public ObjectData GetObjectData( string id )
        {
            return GetData<ObjectData>(id, objectFiles);
        }
        public ObjectData GetMaterialData( string id )
        {
            return GetData<ObjectData>(id, materialFiles);
        }
        public SpawnData GetSpawnData( string id )
        {
            return GetData<SpawnData>(id, spawnListsFiles);
        }
        T GetData<T>( string id, Dictionary<string, DataPointer> dict )
            where T : BasicData, new()
        {
            if( !dict.ContainsKey(id) ) { return null; }

            // XXX: This is the trade off of speed vs memory for either loading and parsing the file
            //      everytime data is needed or keeping all the data objects in memory at all times
            FileInfo file = new FileInfo(dict[id].filePath);
            string json = file.OpenText().ReadToEnd();
            json = json.Substring(dict[id].start, dict[id].length);
            T data = JsonConvert.DeserializeObject<T>(json);

            return data;
        }

        #endregion

        #region Load Methods

        public GameObject LoadObject( string id )
        {
            return LoadObject(GetObjectData(id));
        }
        public GameObject LoadObject( ObjectData data )
        {
            switch( data.type ) {
                case "gameObject":
                    return new GameObject(GameObject.ParseData(data));
                case "physical":
                    return new GameObject(GameObject.ParseData(data));
                case "container":
                    return new Container(Container.ParseData(data));
                default:
                    return null;
            }
        }
        public Material LoadMaterial( string id )
        {
            return LoadMaterial( GetMaterialData(id) );
        }
        public Material LoadMaterial( ObjectData data )
        {
            return new Material( Material.ParseData(data) );
        }

        #endregion
    }

    #region Settings Object

    public class Settings
    {
        public string displayMode = "windowed";
        public int width = 1024;
        public int height = 768;

        public Settings() { }
        public Settings( Settings original )
        {
            this.displayMode = original.displayMode;
            this.width = original.width;
            this.height = original.height;
        }
    }

    #endregion
}
