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
        string nameSpace;

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
        Dictionary<string, DataPointer> spawnFiles;

        List<BasicData> objectDataMemory;
        List<BasicData> materialDataMemory;
        List<BasicData> spawnDataMemory;
        int memoryLength = 50;

        // XXX: Implement hybrid memory/file loading system where recently loaded data objects are stored
        //      in memory using a queue based first-used-first-out system

        #endregion

        #region Initialization Methods

        private DataManager()
        {
            root = $@"{Directory.GetCurrentDirectory()}\..\";
            nameSpace = this.GetType().Namespace + ".";

            scenarioFiles = new Dictionary<string, DataPointer>();
            objectFiles = new Dictionary<string, DataPointer>();
            materialFiles = new Dictionary<string, DataPointer>();
            spawnFiles = new Dictionary<string, DataPointer>();

            objectDataMemory = new List<BasicData>();
            materialDataMemory = new List<BasicData>();
            spawnDataMemory = new List<BasicData>();
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
                        AddFile(file, spawnFiles);
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
            MATERIAL
        }
        private DataType GetTypeId( Type type )
        {
            if( type == (typeof(MaterialData)) || type.IsSubclassOf(typeof(MaterialData)) ) {
                return DataType.MATERIAL;
            } else if( type == (typeof(ObjectData)) || type.IsSubclassOf(typeof(ObjectData)) ) {
                return DataType.OBJECT;
            } else if( type == (typeof(ScenarioData)) || type.IsSubclassOf(typeof(ScenarioData)) ) {
                return DataType.SCENARIO;
            } else if( type == (typeof(SpawnList)) || type.IsSubclassOf(typeof(SpawnList)) ) {
                return DataType.SPAWN;
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
                default:
                    Console.WriteLine($"ERROR: Did not find files for data of type '{type}'");
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
                default:
                    Console.WriteLine($"ERROR: Did not find memory array for data of type '{type}'");
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
                Console.WriteLine($"ERROR: Failed parsing JSON retrieved from '{pointer.filePath}' into JToken: {e}");
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
                    return data;
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
                Console.Write($"ERROR: Failed parsing JSON to data of type '{objectType}': {e}");
            }
            return data;
        }
        private void UpdateMemory( BasicData data, Type type )
        {
            List<BasicData> memory = GetMemory(type);

            memory.Insert(0, data);
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
            Type type = null;
            string typeName = nameSpace + token["type"].Value<string>();
            try {
                type = Type.GetType(typeName);
            } catch( Exception e ) {
                Console.WriteLine($"ERROR: Failed retrieving type '{typeName}': {e}");
            }

            return type;
        }

        #endregion

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
                        Console.Write($"ERROR: Could not parse ScenarioData from file {file.Name}: {e}");
                    }
                }
            }

            return datas.ToArray();
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
                    Console.WriteLine($"ERROR: Failed parsing '{str}' into JSON token: {e}");
                    return null;
                }
            }

            if( token == null ) {
                Console.WriteLine($"ERROR: Failed retrieving token from '{str}'");
                return null;
            }
            return ParseTokenToData(token, type);
        }

        public T LoadObject<T>( string str, Type dataType )
            where T : class
        {
            BasicData data = GetData(str, dataType);
            T obj = null;
            try {
                obj = (T)data.Create();
            } catch( NullReferenceException ) {
                Console.WriteLine($"ERROR: Failed retrieving data from '{str}'");
            } catch( Exception e ) {
                Console.WriteLine($"ERORR: Failed creating instance of type '{typeof(T).Name}' from data of type '{data.GetType()}': {e}");
            }

            return obj;
        }

        /*
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
        }*/

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
