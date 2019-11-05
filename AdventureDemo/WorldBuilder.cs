using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaywardEngine;

namespace AdventureDemo
{
    class WorldBuilder
    {
        Container lastRoom;
        Container lastContainer;

        public Dictionary<string, SpawnList> spawnLists;

        Dictionary<string, Material> materials;

        public WorldBuilder()
        {
            SetupSpawnLists();
            SetupMaterials();
        }
        void SetupSpawnLists()
        {
            spawnLists = new Dictionary<string, SpawnList>();

            // TODO: Spawn lists could theoretically be used to populate entire planets
            //      , however, this would be overwhelming memory. It should be possible to set
            //      spawn lists as 'delayed' and give them a trigger condition (ex: player is able to percieve
            //      weight/volume or look inside). At which point the necessary spawn lists would populate the
            //      relevant containers.
            spawnLists["crew_equipment"] = new SpawnList( new SpawnEntry[] {
                new SpawnEntry( typeof(Physical), new Dictionary<string, object>() {
                    { "name", "uniform" }, { "volume", 2.0 }, { "weight", 6.0 } },
                    0.5, 2, false ),
                new SpawnEntry( typeof(Physical), new Dictionary<string, object>() {
                    { "name", "headset" }, { "volume", 0.4 }, { "weight", 1.0 } },
                    0.2, 1 ),
                new SpawnEntry( typeof(Physical), new Dictionary<string, object>() {
                    { "name", "mints" }, { "volume", 0.01 }, { "weight", 0.01 } },
                    0.2, 4 ),
            });
            spawnLists["crew_quarters"] = new SpawnList( new SpawnEntry[] {
                new SpawnEntry( typeof(Physical), new Dictionary<string, object>() {
                    { "name", "bed" }, { "volume", 100.0 }, { "weight", 200.0 } }, 
                    1.0, 1 ),
                new SpawnEntry( typeof(Physical), new Dictionary<string, object>() {
                    { "name", "desk" }, { "volume", 90.0 }, { "weight", 200.0 } },
                    1.0, 1 ),
                new SpawnEntry( typeof(Container), new Dictionary<string, object>() {
                    { "name", "shelf" }, { "innerVolume", 40.0 }, { "volume", 45.0 }, { "weight", 30.0 } },
                    1.0, 1, true, (container, obj) => {
                        Container shelf = obj as Container;
                        shelf.SpawnContents( spawnLists["crew_equipment"] );

                        return 0;
                    })
            });

            spawnLists["engi_equip"] = new SpawnList( new SpawnEntry[] {
                new SpawnEntry( typeof(Physical), new Dictionary<string, object>() {
                    { "name", "crowbar" }, { "volume", 1.0 }, { "weight", 5.0 } },
                    0.8, 1 ),
                new SpawnEntry( typeof(Physical), new Dictionary<string, object>() {
                    { "name", "wrench" }, { "volume", 0.5 }, { "weight", 2.5 } },
                    0.8, 1 ),
                new SpawnEntry( typeof(Physical), new Dictionary<string, object>() {
                    { "name", "screwdriver" }, { "volume", 0.2 }, { "weight", 1.0 } },
                    0.8, 1 ),
            });
            spawnLists["cargo"] = new SpawnList( new SpawnEntry[] {
                new SpawnEntry( typeof(Physical), new Dictionary<string, object>() {
                    { "name", "steel" }, { "volume", 15.0 }, { "weight", 50.0 } },
                0.8, 10 )
            });
            spawnLists["cargo_bay"] = new SpawnList( new SpawnEntry[] {
                new SpawnEntry( typeof(Container), new Dictionary<string, object>() {
                    { "name", "crate" }, { "innerVolume", 200.0 }, { "volume", 205.0 }, { "weight", 300.0 },
                    { "spawnLists", new SpawnList[] { spawnLists["cargo"] } }, { "spawnNow", true } },
                    0.5, 6 ),
                new SpawnEntry( typeof(Container), new Dictionary<string, object>() {
                    { "name", "toolBox" }, { "innerVolume", 5.0 }, { "volume", 5.2 }, { "weight", 3.0 },
                    { "spawnLists", new SpawnList[] { spawnLists["engi_equip"] } }, { "spawnNow", true } },
                    0.25, 3 )
            });
        }
        void SetupMaterials()
        {
            materials = new Dictionary<string, Material>();

            AddMaterial( "steel", 17, "#6F9BAB" );
            AddMaterial( "glass", 8, "#B5F2FF" );
            AddMaterial( "silicon", 4, "#84B2DF" );
            AddMaterial( "copper", 15, "#D88C17" );
            AddMaterial( "gold", 16, "#F2DB1D" );
            AddMaterial( "flesh", 4, "#D82C7D" );
        }

        public void BuildWorld()
        {
            Container space = new Container("Deep Space", double.PositiveInfinity);
            space.description = "darkness broken up by the dots of lights of distant stars";
            GameManager.instance.AddRoot(space);
            
            SmallSpaceship(space);
            
            Human playerChar = new Human( "Dirk Casirov" );
            playerChar.description = "a mysterious individual";
            FindRoom("spaceship/hallway/cabin").GetContents().Attach(playerChar);

            WaywardWill will = new WaywardWill();
            space.GetContents().Attach(will);
            GameManager.instance.player.Control(will);
        }

        // TODO: Separate this to separate files / load from external file
        void SmallSpaceship(Container container )
        {
            lastContainer = new Container("spaceship", 10000, 15000, 
                Utilities.Pair<Material, double>(materials["steel"], 3),
                Utilities.Pair<Material, double>(materials["glass"], 1)
            );
            container.GetContents().Attach(lastContainer);

            Container hallway = AddRoom( "hallway", lastContainer, 500 );
            AddConnectedRoom( "bridge", 300, "doorway", 100, hallway );
            AddConnectedRoom( "cabin", 200, "doorway", 100, hallway );
            AddConnectedRoom( "engineering", 400, "doorway", 100, hallway );
            AddConnectedRoom( "cargo", 600, "doorway", 100, hallway );
        }

        void LargeSpaceship(Container container)
        {
            lastContainer = new Container("spaceship", 100000, 150000, new KeyValuePair<Material, double>(materials["steel"], 1));
            lastContainer.description = "a craft for travelling between the stars with a gleaming metal hull that protects the fragile internals";

            Container elevator = AddRoom("Main Elevator", lastContainer, 1500); // TODO: Separate elevator shaft and the elevator itself

            OperationsFloorSetup(elevator);
            CrewFloorSetup(elevator);
            EngineeringFloorSetup(elevator);
            CargoFloorSetup(elevator);
        }

        void CrewFloorSetup(Container elevator)
        {
            Container hubRoom = AddConnectedRoom("Quarters Hallway", 1000, "Entrance", 100, elevator);
            hubRoom.description = "a sleek metal hallway connecting the crews' quarters together";
            
            AddConnectedRoom( "Quarters A1", 500, "Doorway", 100, hubRoom, spawnLists["crew_quarters"] );
            AddConnectedRoom( "Quarters A2", 500, "Doorway", 100, hubRoom, spawnLists["crew_quarters"] );
            AddConnectedRoom( "Quarters A3", 500, "Doorway", 100, hubRoom, spawnLists["crew_quarters"] );
            AddConnectedRoom( "Quarters B1", 500, "Doorway", 100, hubRoom, spawnLists["crew_quarters"] );
            AddConnectedRoom( "Quarters B2", 500, "Doorway", 100, hubRoom, spawnLists["crew_quarters"] );
            AddConnectedRoom( "Quarters B3", 500, "Doorway", 100, hubRoom, spawnLists["crew_quarters"] );

            AddConnectedRoom( "Mess Hall", 1500, "Doorway", 100, hubRoom );
            AddConnectedRoom( "Kitchen", 800, "Doorway", 100 );

            hubRoom = AddConnectedRoom( "Crew Escape Pods Hallway", 800, "Doorway", 100, hubRoom );
            AddConnectedRoom( "Crew Escape Pod 1", 400, "Hatch", 70, hubRoom ); // TODO: All pods should be connected using a docking port
            AddConnectedRoom( "Crew Escape Pod 2", 400, "Hatch", 70, hubRoom );
            AddConnectedRoom( "Crew Escape Pod 3", 400, "Hatch", 70, hubRoom );
            AddConnectedRoom( "Crew Escape Pod 4", 400, "Hatch", 70, hubRoom );
        }
        void CargoFloorSetup(Container elevator)
        {
            Container hubRoom = AddConnectedRoom("Cargo Hallway", 1000, "Entrance", 100, elevator);

            AddConnectedRoom( "Cargo Bay 1", 2000, "Bay doorway", 200, hubRoom, spawnLists["cargo_bay"] );
            AddConnectedRoom( "Cargo Bay 2", 2000, "Bay doorway", 200, hubRoom, spawnLists["cargo_bay"] );
            AddConnectedRoom( "Cargo Bay 3", 2000, "Bay doorway", 200, hubRoom, spawnLists["cargo_bay"] );
            AddConnectedRoom( "Cargo Bay 4", 2000, "Bay doorway", 200, hubRoom, spawnLists["cargo_bay"] );
        }
        void OperationsFloorSetup(Container elevator)
        {
            Container hubRoom = AddConnectedRoom("Operations Hallway", 1000, "Entrance", 100, elevator);

            AddConnectedRoom( "Communications", 800, "Doorway", 100, hubRoom );
            AddConnectedRoom( "Sensors", 800, "Doorway", 100, hubRoom );
            AddConnectedRoom( "Armory", 600, "Doorway", 100, hubRoom );
            AddConnectedRoom( "Operations Escape Pod", 400, "Hatch", 70, hubRoom );

            AddConnectedRoom( "Bridge Elevator", 1000, "Entrance", 100, hubRoom );
            AddConnectedRoom( "Bridge", 1000, "Entrance", 100 );
            AddConnectedRoom( "Bridge Escape Pod", 400, "Hatch", 70 );
        }
        void EngineeringFloorSetup(Container elevator)
        {
            Container hubRoom = AddConnectedRoom("Engineering Hallway", 1000, "Entrance", 100, elevator);

            AddConnectedRoom( "Life Support", 1000, "Doorway", 100, hubRoom );
            AddConnectedRoom( "Fabricator", 800, "Doorway", 100, hubRoom );
            AddConnectedRoom( "Hydroponics", 2000, "Doorway", 100, hubRoom );

            AddConnectedRoom( "Engines Elevator", 1000, "Entrance", 100, hubRoom );
            hubRoom = AddConnectedRoom( "Engines Hallway", 1000, "Entrance", 100 );
            AddConnectedRoom( "Engine Block A1", 1200, "Bay doorway", 200, hubRoom );
            AddConnectedRoom( "Engine Block A2", 1200, "Bay doorway", 200, hubRoom );
            AddConnectedRoom( "Engine Block B1", 1200, "Bay doorway", 200, hubRoom );
            AddConnectedRoom( "Engine Block B2", 1200, "Bay doorway", 200, hubRoom );
        }

        public Dictionary<string, Material> GetMaterials()
        {
            return new Dictionary<string, Material>(materials);
        }
        public Material GetMaterial(string key)
        {
            if( !materials.ContainsKey(key) ) { return null; }

            return materials[key];
        }
        public void AddMaterial( string name, double weight, string color = "#ffffff" )
        {
            if( materials.ContainsKey(name) ) { return; }
            materials.Add( name, new Material( name, weight, color ) );
        }

        public Container AddRoom( string name, Container container, double volume, params SpawnList[] spawns)
        {
            lastRoom = new Container( name, volume, volume + 100, Utilities.Pair<Material, double>(materials["steel"], 1) );
            container.GetContents().Attach( lastRoom );
            if( spawns != null ) {
                lastRoom.SpawnContents(spawns, 1, true);
            }

            return lastRoom;
        }
        public Container AddConnectedRoom( string name, double volume, params SpawnList[] spawns )
        {
            return AddConnectedRoom( name, volume, "Opening", 100, spawns );
        }
        public Container AddConnectedRoom( string name, double volume, string connectionName, double connectionVolume, params SpawnList[] spawns )
        {
            return AddConnectedRoom( name, volume, connectionName, connectionVolume, lastRoom, spawns );
        }
        public Container AddConnectedRoom( string name, double volume, string connectionName, double connectionVolume, Container previousRoom, params SpawnList[] spawns )
        {
            Container room = AddRoom( name, lastContainer, volume, spawns );
            room.AddConnection( new Dictionary<string, object> {
                { "second", previousRoom.GetContents() }, { "name", connectionName },
                { "throughput", connectionVolume }
            });

            return room;
        }

        public Container FindRoom( string path )
        {
            for( int i = 0; i < GameManager.instance.RootCount(); i++ ) {
                Container root = GameManager.instance.GetRoot(i) as Container;
                if( root == null ) { continue; }

                Container found = FindRoom( root, path );
                if( found != null ) { return found; }
            }

            return null;
        }
        public Container FindRoom( Container from, string path )
        {
            if( from == null ) { return null; }

            if( from.GetData("name").text == path ) {
                if( from != null ) {
                    return from;
                }
            }

            int indexOfStep = path.IndexOf('/');
            string nextStep = indexOfStep != -1 ? path.Substring(0, indexOfStep) : path;
            string nextPath = path.Substring(indexOfStep+1);

            foreach( Connection connection in from.GetConnections() ) {
                string roomName = connection.secondContainer.GetParent().GetData("name").text;
                if( roomName == nextStep ) {
                    Container found = FindRoom(connection.secondContainer.GetParent() as Container, nextPath);
                    if( found != null ) { return found; }
                }
            }
            for( int i = 0; i < from.GetContentCount(); i++ ) {
                GameObject obj = from.GetContent(i);
                if( obj.GetData("name").text == nextStep ) {
                    Container container = obj as Container;
                    if( container != null ) {
                        Container found = FindRoom(container, nextPath);
                        if( found != null ) { return found; }
                    }
                }
            }

            return null;
        }
    }

    // TODO: Improve constructor formats to make spawnlist instantiation less wordy
    delegate int SpawnDelegate( Container container );
    class SpawnList
    {
        List<SpawnEntry> spawns;
        SpawnDelegate spawnAction;

        public SpawnList( SpawnEntry[] entries, SpawnDelegate action = null )
        {
            spawns = new List<SpawnEntry>(entries);
            spawnAction = action;
        }

        /// <summary>
        /// Initiates the spawn list populating the container with its entries.
        /// </summary>
        /// <param name="container">Container to be populated.</param>
        /// <param name="weight">Multiplier affecting the chances of spawns. Can be used to represent plentiful/scarce situations.</param>
        /// <returns></returns>
        public Dictionary<string, int> Spawn( Container container, double weight = 1 )
        {
            Dictionary<string, int> spawnAmounts = new Dictionary<string, int>();

            foreach( SpawnEntry entry in spawns ) {
                spawnAmounts[entry.type.Name] = entry.Spawn(container, weight);
            }

            if( spawnAction != null ) { spawnAmounts["action"] += spawnAction(container); }

            return spawnAmounts;
        }
    }

    delegate int SpawnEntryDelegate( Container container, GameObject obj );
    class SpawnEntry
    {
        public Type type;
        public Dictionary<string, object> data;
        public double spawnChance;
        public int spawnQuantity;
        public bool independantSpawn;
        public SpawnEntryDelegate spawnAction;

        /// <summary>
        /// An entry in a SpawnList. Used to randomize contents of the world.
        /// </summary>
        /// <param name="t">Type of object to spawn. Must be derived of GameObject./param>
        /// <param name="d">Parameters to be passed to the constructor.</param>
        /// <param name="chance">Chance of spawn (0 - 1).</param>
        /// <param name="quantity">Amount of objects to try to spawn.</param>
        /// <param name="independant">If True all objects will roll separately for spawn chance. If False, any failure stops spawning (Default: True).</param>
        public SpawnEntry( Type t, Dictionary<string, object> d, double chance, int quantity, bool independant = true, SpawnEntryDelegate action = null )
        {
            if( !t.IsSubclassOf(typeof(GameObject)) ) {
                throw new System.InvalidCastException("Attempted to instantiate SpawnEntry with a type not derived from GameObject");
            }

            type = t;
            data = d;
            spawnChance = chance;
            spawnQuantity = quantity;
            independantSpawn = independant;
            spawnAction = action;
        }

        public int Spawn( Container container, double weight = 1 )
        {
            data["container"] = container;

            int amountSpawned = 0;

            for( int i = 0; i < spawnQuantity; i++ ) {
                double roll = WaywardManager.instance.random.NextDouble() * weight;
                if( roll <= spawnChance ) {
                    GameObject obj = Activator.CreateInstance(type, data) as GameObject;
                    amountSpawned++;

                    if( spawnAction != null ) { amountSpawned += spawnAction(container, obj); }

                } else if( !independantSpawn ){
                    break;
                }
            }

            return amountSpawned;
        }
    }
}
