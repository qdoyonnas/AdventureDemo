using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdventureCore
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

        WaywardEngine.WaywardManager waywardManager;

        string root;
        public string nameSpace;

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
        Dictionary<string, DataPointer> worldFiles;
        Dictionary<string, DataPointer> scenarioFiles;
        Dictionary<string, DataPointer> objectFiles;
        Dictionary<string, DataPointer> materialFiles;
        Dictionary<string, DataPointer> spawnFiles;
        Dictionary<string, DataPointer> verbFiles;
        Dictionary<string, DataPointer> behaviourFiles;

        List<BasicData> objectDataMemory;
        List<BasicData> materialDataMemory;
        List<BasicData> spawnDataMemory;
        List<BasicData> verbDataMemory;
        List<BasicData> behaviourDataMemory;
        int memoryLength = 50;

        // XXX: Implement hybrid memory/file loading system where recently loaded data objects are stored
        //      in memory using a queue based first-used-first-out system

        #endregion

        #region Initialization Methods

        private DataManager()
        {
            waywardManager = WaywardEngine.WaywardManager.instance;

            root = $@"{Directory.GetCurrentDirectory()}\..\";
            nameSpace = this.GetType().Namespace + ".";

            worldFiles = new Dictionary<string, DataPointer>();
            scenarioFiles = new Dictionary<string, DataPointer>();
            objectFiles = new Dictionary<string, DataPointer>();
            materialFiles = new Dictionary<string, DataPointer>();
            spawnFiles = new Dictionary<string, DataPointer>();
            verbFiles = new Dictionary<string, DataPointer>();
            behaviourFiles = new Dictionary<string, DataPointer>();

            objectDataMemory = new List<BasicData>();
            materialDataMemory = new List<BasicData>();
            spawnDataMemory = new List<BasicData>();
            verbDataMemory = new List<BasicData>();
            behaviourDataMemory = new List<BasicData>();
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
        private void LoadDirectory( string subPath )
        {
            DirectoryInfo directory = new DirectoryInfo(dataDirectory.FullName + "\\" + subPath);
            if( !directory.Exists ) { directory.Create(); return; }

            LoadDirectory(directory);
        }
        private void LoadDirectory( DirectoryInfo directory )
        {
            FileInfo[] files = directory.GetFiles();
            foreach( FileInfo file in files ) {
                switch( file.Extension ) {
                    case ".world":
                        AddFile(file, worldFiles);
                        break;
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
                        AddFile(file, spawnFiles);
                        break;
                    case ".verb":
                        AddFile(file, verbFiles);
                        break;
                    case ".behaviour":
                        AddFile(file, behaviourFiles);
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
                waywardManager.Log($@"<red>ERROR: Could not parse file {file.Name}: {e}</red>");
            }
        }

        #endregion

        #region Data Methods

        #region Data Utility Methods

        public bool IsId( string str )
        {
            str = str.Trim();
            return str[0] != '{' && str[0] != '[';
        }

        private enum DataType {
            UNKNOWN,
            SCENARIO,
            OBJECT,
            SPAWN,
            MATERIAL,
            VERB,
            BEHAVIOUR
        }
        private DataType GetTypeId( Type type )
        {
            if( typeof(MaterialData).IsAssignableFrom(type) ) {
                return DataType.MATERIAL;
            } else if( typeof(ObjectData).IsAssignableFrom(type) ) {
                return DataType.OBJECT;
            } else if( typeof(ScenarioData).IsAssignableFrom(type) ) {
                return DataType.SCENARIO;
            } else if( typeof(SpawnList).IsAssignableFrom(type) ) {
                return DataType.SPAWN;
            } else if( typeof(VerbData).IsAssignableFrom(type) ) {
                return DataType.VERB;
            } else if( typeof(BehaviourData).IsAssignableFrom(type) ) {
                return DataType.BEHAVIOUR;
            } else {
                return DataType.UNKNOWN;
            }
        }
        private Dictionary<string, DataPointer> GetFiles( Type type )
        {
            switch( GetTypeId(type) ) {
                case DataType.MATERIAL:
                    return materialFiles;
                case DataType.OBJECT:
                    return objectFiles;
                case DataType.SCENARIO:
                    return scenarioFiles;
                case DataType.SPAWN:
                    return spawnFiles;
                case DataType.VERB:
                    return verbFiles;
                case DataType.BEHAVIOUR:
                    return behaviourFiles;
                default:
                    waywardManager.Log($@"<red>ERROR: Did not find files for data of type '{type}'</red>");
                    return null;
            }
        }
        private List<BasicData> GetMemory( Type type )
        {
            switch( GetTypeId(type) ) {
                case DataType.MATERIAL:
                    return materialDataMemory;
                case DataType.OBJECT:
                    return objectDataMemory;
                case DataType.SCENARIO:
                    return null;
                case DataType.SPAWN:
                    return spawnDataMemory;
                case DataType.VERB:
                    return verbDataMemory;
                case DataType.BEHAVIOUR:
                    return behaviourDataMemory;
                default:
                    waywardManager.Log($@"<red>ERROR: Did not find memory array for data of type '{type}'</red>");
                    return null;
            }
        }

        private delegate void ForEachJTokenAction( JToken token );
        private void ForEachJToken( JToken token, ForEachJTokenAction action )
        {
            if( action == null ) { return; }

            if( token.Type == JTokenType.Array ) {
                foreach( JToken t in token ) {
                    action(t);
                }
            } else {
                action(token);
            }
        }

        private JToken GetJSONFromFile( string id, Type type )
        {
            Dictionary<string, DataPointer> files = GetFiles(type);
            if( files == null || !files.ContainsKey(id) ) { return null; }

            DataPointer pointer = files[id];
            StreamReader file = new StreamReader(pointer.filePath);
            JToken token = null;
            try {
                token = JToken.Parse(file.ReadToEnd());
            } catch( Exception e ) {
                waywardManager.Log($@"<red>ERROR: Failed parsing JSON retrieved from '{pointer.filePath}' into JToken: {e}</red>");
                return null;
            }

            if( token != null && token.Type == JTokenType.Array ) {
                return ((JArray)token)[pointer.index];
            }

            return token;
        }

        private BasicData RetrieveDataFromMemory( string id, Type type )
        {
            List<BasicData> memory = GetMemory(type);
            if( memory == null ) { return null; }
            foreach( BasicData data in memory ) {
                if( data.id == id ) {
                    Type dataType = data.GetType();
                    BasicData copiedData = (BasicData)Activator.CreateInstance(dataType, data);

                    return copiedData;
                }
            }

            return null;
        }

        private BasicData ParseTokenToData( JToken token, Type type )
        {
            if( token == null ) { return null; }

            if( token["basedOn"] != null && token["basedOn"].Type != JTokenType.Null ) {
                return GenerateBasedData(token, type);
            }

            BasicData data = null;
            Type objectType = type;
            if( token["type"] != null && token["type"].Type != JTokenType.Null ) {
                objectType = GetTypeFromJSON(token);
                if( objectType == null ) { objectType = type; }
            }

            try {
                data = (BasicData)token.ToObject(objectType);
                UpdateMemory(data, type);
            } catch( Exception e ) {
                waywardManager.Log($@"<red>ERROR: Failed parsing JSON to data of type '{objectType}': {e}</red>");
            }
            return data;
        }
        private void UpdateMemory( BasicData data, Type type )
        {
            List<BasicData> memory = GetMemory(type);

            Type dataType = data.GetType();
            BasicData copiedData = (BasicData)Activator.CreateInstance(dataType, data);
            memory.Insert(0, copiedData);
            if( memory.Count > memoryLength ) {
                memory.RemoveAt(memory.Count - 1);
            }
        }

        private BasicData GenerateBasedData( JToken token, Type type )
        {
            BasicData data = GetData(token["basedOn"].ToString(), type);
            JsonReader reader = token.CreateReader();
            JsonSerializer.CreateDefault().Populate(reader, data);

            return data;
        }
        private Type GetTypeFromJSON(JToken token)
        {
            string typeName = token["type"].Value<string>();

            return GetTypeFromString(typeName);
        }
        private Type GetTypeFromString( string str )
        {
            Type type = null;
            string typeName = nameSpace + str;
            try {
                type = Type.GetType(typeName);
            } catch( Exception e ) {
                waywardManager.Log($@"<red>ERROR: Failed retrieving type '{typeName}': {e}</red>");
            }

            return type;
        }

        #endregion

        public WorldData[] GetWorldDatas()
        {
            List<WorldData> datas = new List<WorldData>();

            List<string> files = new List<string>();
            foreach( DataPointer pointer in worldFiles.Values ) {
                if( !files.Contains(pointer.filePath) ) {
                    files.Add(pointer.filePath);

                    FileInfo file = new FileInfo(pointer.filePath);
                    try {
                        JToken jToken = JToken.Parse(file.OpenText().ReadToEnd());
                        ForEachJToken(jToken, ( token ) => {
                            WorldData data = token.ToObject<WorldData>();
                            datas.Add(data);
                        });
                    } catch( Exception e ) {
                        waywardManager.Log($@"<red>ERROR: Could not parse WorldData from file {file.Name}: {e}</red>");
                    }
                }
            }

            return datas.ToArray();
        }
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
                        ForEachJToken(jToken, ( token ) => {
                            ScenarioData data = token.ToObject<ScenarioData>();
                            datas.Add(data);
                        });
                    } catch( Exception e ) {
                        waywardManager.Log($@"<red>ERROR: Could not parse ScenarioData from file {file.Name}: {e}</red>");
                    }
                }
            }

            return datas.ToArray();
        }
        public ScenarioData[] GetScenarioDatas( WorldData worldData )
        {
            ScenarioData[] datas = GetScenarioDatas();
            List<ScenarioData> filteredDatas = new List<ScenarioData>();

            foreach( ScenarioData data in datas ) {
                if( data.world == worldData.id ) {
                    filteredDatas.Add(data);
                }
            }

            return filteredDatas.ToArray();
        }

        public BasicData GetData( string str, Type type )
        {
            JToken token;
            if( IsId(str) ) {
                BasicData data = RetrieveDataFromMemory(str, type);
                if( data != null ) { return data; }
                token = GetJSONFromFile(str, type);
            } else {
                try {
                    token = JToken.Parse(str);
                } catch( Exception e ) {
                    waywardManager.Log($@"<red>ERROR: Failed parsing '{str}' into JSON token: {e}</red>");
                    return null;
                }
            }

            if( token == null ) {
                waywardManager.Log($@"<red>ERROR: Failed retrieving token from '{str}'</red>");
                return null;
            }
            return ParseTokenToData(token, type);
        }
        public T LoadObject<T>( string str, Type dataType, Dictionary<string, object> context = null )
            where T : class
        {
            BasicData data = GetData(str, dataType);
            T obj = null;
            try {
                obj = (T)data.Create(context);
            } catch( NullReferenceException e ) {
                waywardManager.Log($@"<red>ERROR: Failed retrieving data from '{str}': {e}</red>");
            } catch( Exception e ) {
                waywardManager.Log($@"<red>ERORR: Failed creating instance of type '{typeof(T).Name}' from data of type '{data.GetType()}': {e}</red>");
            }

            return obj;
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
