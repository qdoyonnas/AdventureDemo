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

        string root;

        DirectoryInfo dataDirectory;

        Settings settings;
        FileInfo settingsFile;

        Dictionary<string, string> scenarioFiles;
        Dictionary<string, string> objectFiles;
        Dictionary<string, string> spawnListsFiles;

        private DataManager()
        {
            root = $@"{Directory.GetCurrentDirectory()}\..\";

            scenarioFiles = new Dictionary<string, string>();
            objectFiles = new Dictionary<string, string>();
            spawnListsFiles = new Dictionary<string, string>();
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

        void SetupData()
        {
            dataDirectory = new DirectoryInfo(root + "Data");
            if( !dataDirectory.Exists ) {
                dataDirectory.Create();
            }

            LoadDirectory("Scenarios");
            LoadDirectory("Objects");
            LoadDirectory("SpawnLists");
        }
        public void LoadDirectory( string subPath )
        {
            DirectoryInfo directory = new DirectoryInfo(dataDirectory.FullName + "\\" + subPath);
            if( !directory.Exists ) { directory.Create(); return; }

            FileInfo[] files = directory.GetFiles();
            foreach( FileInfo file in files ) {
                switch( file.Extension ) {
                    case ".scenario":
                        AddFile<ScenarioData>(file, scenarioFiles);
                        break;
                    case ".object":
                        AddFile<ObjectData>(file, objectFiles);
                        break;
                    case ".spawn":
                        AddFile<SpawnData>(file, spawnListsFiles);
                        break;
                }
            }
        }
        void AddFile<T>( FileInfo file, Dictionary<string, string> dict )
            where T : BasicData
        {
            List<T> datas = ParseFile<T>(file);

            foreach( T data in datas ) {
                dict.Add(data.id, file.FullName);
            }
        }

        List<T> ParseFile<T>( string path )
        {
            return ParseFile<T>( new FileInfo(path));
        }
        List<T> ParseFile<T>( FileInfo file )
        {
            if( !file.Exists ) { return new List<T>(); }

            StreamReader reader = file.OpenText();
            string contents = reader.ReadToEnd();

            List<T> datas = new List<T>();
            int subIndex;
            do {
                subIndex = contents.IndexOf("};");
                if( subIndex == -1 ) { break; }

                string json = contents.Substring(0, subIndex + 1);
                contents = contents.Remove(0, subIndex + 2);
                datas.Add(JsonConvert.DeserializeObject<T>(json));
            } while( subIndex != -1 );

            return datas;
        }

        public ScenarioData[] GetScenarios()
        {
            List<ScenarioData> datas = new List<ScenarioData>();

            List<string> files = new List<string>();
            foreach( string file in scenarioFiles.Values ) {
                if( !files.Contains(file) ) {
                    files.Add(file);

                    datas.AddRange( ParseFile<ScenarioData>(file) );
                }
            }

            return datas.ToArray();
        }
        public ScenarioData LoadScenario( string id )
        {
            return LoadData<ScenarioData>(id, scenarioFiles);
        }
        public ObjectData LoadObject( string id )
        {
            return LoadData<ObjectData>(id, objectFiles);
        }
        public SpawnData LoadSpawnList( string id )
        {
            return LoadData<SpawnData>(id, spawnListsFiles);
        }
        T LoadData<T>( string id, Dictionary<string, string> dict )
            where T : BasicData, new()
        {
            if( !dict.ContainsKey(id) ) { return null; }

            // XXX: This is the trade off of speed vs memory for either loading and parsing the file
            //      everytime data is needed or keeping all the data objects in memory at all times
            FileInfo file = new FileInfo(dict[id]);
            List<T> datas = ParseFile<T>(file);

            foreach( T d in datas ) {
                if( d.id == id ) {
                    return d;
                }
            }

            return null;
        }
    }

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
}
