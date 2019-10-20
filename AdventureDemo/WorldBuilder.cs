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

        public WorldBuilder()
        {

        }

        public void BuildWorld()
        {
            Container space = new Container("Deep Space", null, double.PositiveInfinity);
            space.description = "darkness broken up by the dots of lights of distant stars";
            lastContainer = new Container("Spaceship", space, 100000, 150000, 50*10^8);
            lastContainer.description = "a craft for travelling between the stars with a gleaming metal hull that protects the fragile internals";

            Container elevator = AddRoom("Main Elevator", lastContainer, 1500); // TODO: Separate elevator shaft and the elevator itself

            OperationsFloorSetup(elevator);
            CrewFloorSetup(elevator);
            EngineeringFloorSetup(elevator);
            CargoFloorSetup(elevator);
            
            Character playerChar = new Character( "You", FindRoom(elevator, "Quarters Hallway/Quarters A3"), 2.5, 65, 150 );
            playerChar.description = "a mysterious individual";
            GameManager.instance.player.Control(playerChar);
        }
        void CrewFloorSetup(Container elevator)
        {
            Container hubRoom = AddConnectedRoom("Quarters Hallway", 1000, "Entrance", 100, elevator);
            hubRoom.description = "a sleek metal hallway connecting the crew's quarters together";
            
            AddConnectedRoom( "Quarters A1", 500, "Doorway", 100, hubRoom );
            AddConnectedRoom( "Quarters A2", 500, "Doorway", 100, hubRoom );
            AddConnectedRoom( "Quarters A3", 500, "Doorway", 100, hubRoom );
            AddConnectedRoom( "Quarters B1", 500, "Doorway", 100, hubRoom );
            AddConnectedRoom( "Quarters B2", 500, "Doorway", 100, hubRoom );
            AddConnectedRoom( "Quarters B3", 500, "Doorway", 100, hubRoom );

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

            AddConnectedRoom( "Cargo Bay 1", 2000, "Bay doorway", 200, hubRoom );
            AddConnectedRoom( "Cargo Bay 2", 2000, "Bay doorway", 200, hubRoom );
            AddConnectedRoom( "Cargo Bay 3", 2000, "Bay doorway", 200, hubRoom );
            AddConnectedRoom( "Cargo Bay 4", 2000, "Bay doorway", 200, hubRoom );
            AddConnectedRoom( "Cargo Escape Pod", 400, "Hatch", 70, hubRoom );
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

        public Container AddRoom( string name, Container container, double volume )
        {
            return lastRoom = new Container( name, container, volume, volume + 50 );
        }
        public Container AddConnectedRoom( string name, double volume )
        {
            return AddConnectedRoom( name, volume, "Opening", 100 );
        }
        public Container AddConnectedRoom(  string name, double volume, string connectionName, double connectionVolume )
        {
            return AddConnectedRoom( name, volume, connectionName, connectionVolume, lastRoom );
        }
        public Container AddConnectedRoom(  string name, double volume, string connectionName, double connectionVolume, Container previousRoom )
        {
            Container room = AddRoom( name, lastContainer, volume );
            room.AddConnection(new PhysicalConnection(connectionName, room, previousRoom, connectionVolume), true);

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
        public Container FindRoom( IContainer from, string path )
        {
            if( from.GetData("name").text == path ) {
                Container fromContainer = from as Container;
                if( fromContainer != null ) {
                    return fromContainer;
                }
            }

            int indexOfStep = path.IndexOf('/');
            string nextStep = indexOfStep != -1 ? path.Substring(0, indexOfStep) : path;
            string nextPath = path.Substring(indexOfStep+1);

            foreach( Connection connection in from.GetConnections() ) {
                Console.WriteLine(connection.secondContainer.GetData("name").text);
                if( connection.secondContainer.GetData("name").text == nextStep ) {
                    Container found = FindRoom(connection.secondContainer, nextPath);
                    if( found != null ) { return found; }
                }
            }
            for( int i = 0; i < from.ContentCount(); i++ ) {
                GameObject obj = from.GetContent(i);
                Console.WriteLine(obj.GetData("name").text);
                if( obj.GetData("name").text == nextStep ) {
                    IContainer container = obj as IContainer;
                    if( container != null ) {
                        Container found = FindRoom(container, nextPath);
                        if( found != null ) { return found; }
                    }
                }
            }

            return null;
        }
    }
}
