using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            public int index;

            public DataPointer( string p, int i )
            {
                filePath = p;
                index = i;
            }
        }
        Dictionary<string, DataPointer> scenarioFiles;
        Dictionary<string, DataPointer> objectFiles;
        Dictionary<string, DataPointer> materialFiles;
        Dictionary<string, DataPointer> spawnListsFiles;

        List<ObjectData> objectDataMemory;
        List<MaterialData> materialDataMemory;
        List<SpawnData> spawnDataMemory;
        int objectMemoryLength = 50;
        int materialMemoryLength = 50;
        int spawnMemoryLength = 50;

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

            objectDataMemory = new List<ObjectData>();
            materialDataMemory = new List<MaterialData>();
            spawnDataMemory = new List<SpawnData>();
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

        void SetupData()
        {
            dataDirectory = new DirectoryInfo(root + "Data");
            if( !dataDirectory.Exists ) {
                dataDirectory.Create();
            }

            LoadDirectory(dataDirectory);
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
                        AddFile(file, scenarioFiles);
                        break;
                    case ".object":
                        AddFile(file, objectFiles);
                        break;
                    case ".material":
                        AddFile(file, materialFiles);
                        break;
                    case ".spawn":
                        AddFile(file, spawnListsFiles);
                        break;
                }
            }

            foreach( DirectoryInfo dir in directory.GetDirectories() ) {
                LoadDirectory(dir);
            }
        }

        void AddFile( FileInfo file, Dictionary<string, DataPointer> dict )
        {
            try {
                JToken jToken = JToken.Parse(file.OpenText().ReadToEnd());
                JArray jArray = jToken as JArray;
                if( jArray != null ) {
                    for( int i = 0; i < jArray.Count; i++ ) {
                        dict.Add(jArray[i]["id"].ToString(), new DataPointer(file.FullName, i));
                    }
                } else {
                    dict.Add(jToken["id"].ToString(), new DataPointer(file.FullName, 0));
                }
            } catch( Exception e ) {
                Console.Write($"ERROR: Could not parse file {file.Name}: {e}");
            }
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

                    FileInfo file = new FileInfo(pointer.filePath);
                    try {
                        JToken jToken = JToken.Parse(file.OpenText().ReadToEnd());
                        if( jToken.Type == JTokenType.Array ) {
                            ScenarioData[] fileData = jToken.ToObject<ScenarioData[]>();
                            foreach( ScenarioData data in fileData ) {
                                if( data != null ) { datas.Add(data); }
                            }
                        } else {
                            ScenarioData data = jToken.ToObject<ScenarioData>();
                            if( data != null ) { datas.Add(data); }
                        }
                    } catch( Exception e ) {
                        Console.Write($"ERROR: Could not parse ScenarioData from file {file.Name}: {e}");
                    }
                }
            }

            return datas.ToArray();
        }
        public ScenarioData GetScenarioData( string id )
        {
            JToken token = RetrieveDataFromFile<ScenarioData>(id, scenarioFiles);
            return GetScenarioData(token);
        }
        public ScenarioData GetScenarioData( JToken token )
        {
            if( token.Type == JTokenType.String ) { return GetScenarioData(token.Value<string>()); }

            ScenarioData data = null;

            try {
                data = token.ToObject<ScenarioData>();
            } catch( Exception e ) {
                Console.WriteLine($"ERROR: Failed converting JSON into ScenarioData: {e}");
            }

            return data;
        }

        public ObjectData GetObjectData( string id )
        {
            ObjectData data = RetrieveDataFromMemory<ObjectData>(id, objectDataMemory);
            if( data != null ) { return data; }

            JToken token = RetrieveDataFromFile<ObjectData>(id, objectFiles);
            return GetObjectData(token);
        }
        public ObjectData GetObjectData( JToken token )
        {
            if( token.Type == JTokenType.String ) { return GetObjectData(token.Value<string>()); }

            ObjectData data = null;

            try {
                if( token["basedOn"] != null && token["basedOn"].Type != JTokenType.Null ) {
                    ObjectData basedData = GetObjectData(token["basedOn"]);
                    data = basedData;
                    JsonReader reader = token.CreateReader();
                    JsonSerializer.CreateDefault().Populate(reader, data);
                } else {
                    string typeName = "AdventureDemo." + token["type"].Value<string>();
                    Type objectType = Type.GetType(typeName);
                    data = (ObjectData)token.ToObject(objectType);
                }
            } catch( Exception e ) {
                Console.WriteLine($"ERROR: Failed converting JSON into ObjectData: {e}");
            }

            if( data != null ) {
                UpdateObjectMemory(data);
            }
            return data;
        }
        public void UpdateObjectMemory( ObjectData data )
        {
            objectDataMemory.Insert(0, data);
            if( objectDataMemory.Count > objectMemoryLength ) {
                objectDataMemory.RemoveAt(objectDataMemory.Count - 1);
            }
        }

        public MaterialData GetMaterialData( string id )
        {
            MaterialData data = RetrieveDataFromMemory<MaterialData>(id, materialDataMemory);
            if( data != null ) { return data; }

            JToken token = RetrieveDataFromFile<MaterialData>(id, objectFiles);
            return GetMaterialData(token);
        }
        public MaterialData GetMaterialData( JToken token )
        {
            if( token.Type == JTokenType.String ) { return GetMaterialData(token.Value<string>()); }



            return null;
        }

        public JToken RetrieveDataFromFile<T>( string id, Dictionary<string, DataPointer> files )
            where T : BasicData
        {
            if( !files.ContainsKey(id) ) { return null; } // XXX: Look for new file?

            DataPointer pointer = objectFiles[id];

            FileInfo file = new FileInfo(pointer.filePath);
            try {
                JToken jObject;
                if( pointer.index != 0 ) {
                    JArray jArray = JArray.Parse(file.OpenText().ReadToEnd());
                    jObject = jArray[pointer.index];
                } else {
                    jObject = JToken.Parse(file.OpenText().ReadToEnd());
                }

                return jObject;
            } catch( Exception e ) {
                Console.WriteLine($"ERROR: Failed retrieving data with id '{id}': {e}");
                return null;
            }
        }
        public T RetrieveDataFromMemory<T>( string id, List<T> memory )
            where T : BasicData
        {
            foreach( T data in memory ) {
                if( data.id == id ) {
                    return data;
                }
            }

            return null;
        }

        /// <summary>
        /// Searches all files and memory lists. SLOW!
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BasicData GetData( string id )
        {
            BasicData data;
            data = GetObjectData(id);
            if( data != null ) { return data; }

            data = GetMaterialData(id);
            if( data != null ) { return data; }

            data = GetScenarioData(id);
            if( data != null ) { return data; }

            return null;
        }
        /// <summary>
        /// Searches all files and memory lists. SLOW!
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BasicData GetData( JToken token )
        {
            if( token.Type == JTokenType.String ) { return GetData(token.Value<string>()); }

            BasicData data;
            data = GetObjectData(token);
            if( data != null ) { return data; }

            data = GetMaterialData(token);
            if( data != null ) { return data; }

            data = GetScenarioData(token);
            if( data != null ) { return data; }

            return null;
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
