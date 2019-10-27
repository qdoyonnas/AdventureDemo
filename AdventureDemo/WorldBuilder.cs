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

        Dictionary<string, SpawnList> spawnLists;

        public WorldBuilder()
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

        public void BuildWorld()
        {
            Container space = new Container("Deep Space", null, double.PositiveInfinity);
            space.description = "darkness broken up by the dots of lights of distant stars";
            lastContainer = new Container("spaceship", space.GetContents(), 100000, 150000, 50*10^8);
            lastContainer.description = "a craft for travelling between the stars with a gleaming metal hull that protects the fragile internals";

            Container elevator = AddRoom("Main Elevator", lastContainer, 1500); // TODO: Separate elevator shaft and the elevator itself

            OperationsFloorSetup(elevator);
            CrewFloorSetup(elevator);
            EngineeringFloorSetup(elevator);
            CargoFloorSetup(elevator);
            
            Character playerChar = new Character( "You", elevator.GetContents(), 5, 65, 150 );
            playerChar.description = "a mysterious individual";
            GameManager.instance.player.Control(playerChar);
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
            AddConnectedRoom( "Crew Escape Pod 1", 400, "Hatch", 70, hubRoom, spawnLists["cargo_bay"] ); // TODO: All pods should be connected using a docking port
            AddConnectedRoom( "Crew Escape Pod 2", 400, "Hatch", 70, hubRoom, spawnLists["cargo_bay"] );
            AddConnectedRoom( "Crew Escape Pod 3", 400, "Hatch", 70, hubRoom, spawnLists["cargo_bay"] );
            AddConnectedRoom( "Crew Escape Pod 4", 400, "Hatch", 70, hubRoom, spawnLists["cargo_bay"] );
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

        public Container AddRoom( string name, Container container, double volume, params SpawnList[] spawns)
        {
            lastRoom = new Container( name, container.GetContents(), volume, volume + 50 );
            if( spawns != null ) {
                lastRoom.SpawnContents(spawns, 1, true);
            }

            return lastRoom;
        }
        public Container AddConnectedRoom( string name, double volume, SpawnList[] spawns = null )
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

            return room;
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
