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

            Container elevator = AddRoom("Main Elevator", lastContainer, 1500);

            Container hubRoom = AddConnectedRoom("Quarters Hallway", 1000, "Entrance", 100);
            hubRoom.description = "a sleek metal hallway connecting the crew's quarters together";
            
            AddConnectedRoom( "Quarters A1", 500, "Doorway", 100, hubRoom );
            AddConnectedRoom( "Quarters A2", 500, "Doorway", 100, hubRoom );
            AddConnectedRoom( "Quarters A3", 500, "Doorway", 100, hubRoom );
            AddConnectedRoom( "Quarters B1", 500, "Doorway", 100, hubRoom );
            AddConnectedRoom( "Quarters B2", 500, "Doorway", 100, hubRoom );
            AddConnectedRoom( "Quarters B3", 500, "Doorway", 100, hubRoom );

            AddConnectedRoom( "Mess Hall", 1500, "Doorway", 100, hubRoom );
            AddConnectedRoom( "Kitchen", 800, "Doorway", 100 );

            hubRoom = AddConnectedRoom("Cargo Hallway", 1000, "Entrance", 100, elevator);

            AddConnectedRoom("Cargo Bay 1", 2000, "Bay doors", 200, hubRoom);
            AddConnectedRoom("Cargo Bay 2", 2000, "Bay doors", 200, hubRoom);
            AddConnectedRoom("Cargo Bay 3", 2000, "Bay doors", 200, hubRoom);
            AddConnectedRoom("Cargo Bay 4", 2000, "Bay doors", 200, hubRoom);

            Character playerChar = new Character( "You", FindRoom(elevator, "Quarters Hallway/Quarteres A3"), 2.5, 65, 150 );
            playerChar.description = "a mysterious individual";
            GameManager.instance.player.Control(playerChar);
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
            room.AddConnection(new PhysicalConnection(connectionName, room, previousRoom, connectionVolume));

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
            if( from.GetData("name").text == path ) { return from; }

            int indexOfStep = path.IndexOf('/');
            string nextStep = path.Substring(0, indexOfStep);
            Console.WriteLine("NextStep: " + nextStep);
            string nextPath = path.Substring(indexOfStep+1, 0);
            Console.WriteLine("NextPath: " + nextPath);

            foreach( Connection connection in from.GetConnections() ) {
                if( connection.secondContainer.GetData("name").text == nextStep ) {

                }
            }

            return null;
        }
    }
}
