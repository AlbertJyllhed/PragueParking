using System;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace PragueParking1
{
    public class Program
    {
        static void Main()
        {
            string[] parkingGarage = new string[100];
            string input;

            do
            {
                input = DisplayMenu(["What do you want to do?\n", "[1]: Find vacant parking space", "[2]: Search for a vehicle",
                    "[3]: Display a parking space", "[4]: Display the parking lot", "[+]: Exit application\n"]);

                switch (input)
                {
                    case "1":
                        FindParkingSpace();
                        break;
                    case "2":
                        FindVehicle();
                        break;
                    case "3":
                        DisplayParkingSpace();
                        break;
                    case "4":
                        DisplayParkingLot();
                        break;
                    case "+":
                        Console.Clear();
                        Console.WriteLine("Exiting application...");
                        break;
                }
            }
            while (input != "+");


            //displays some text for the user
            string DisplayMenu(string[] messages)
            {
                Console.Clear();

                foreach (string message in messages)
                {
                    Console.WriteLine(message);
                }

                string input = InputValidation.EvaluateInputString();
                return input;
            }


            void DisplayMessage(string[] messages)
            {
                foreach (string message in messages)
                {
                    Console.WriteLine(message);
                }

                Console.WriteLine("\nPress enter to return to main menu.\n");
                Console.ReadKey();
            }


            void FindParkingSpace()
            {
                //validation methods take the input and makes sure its valid, if its not you have to type it in again
                string reg = DisplayMenu(["Type the registration number for your vehicle.\n", "Reg number example: ABC123\n"]);
                reg = InputValidation.ValidateRegNumber(reg);

                input = DisplayMenu(["Are you parking a car or a motorcycle?\n", "[1]: Park a car", "[2]: Park a motorcycle\n"]);
                string vehicleType = InputValidation.ValidateVehicleType(input);

                input = DisplayMenu(["Where would you like to park your vehicle?\n", "Type a number between 1-100 to choose a parking space\n"]);
                int index = InputValidation.ValidateIndex(parkingGarage, input);
                index = FindVacantParkingSpace(vehicleType, index);

                string vehicle = CreateVehicle(vehicleType, reg);
                AddVehicle(vehicle, index);

                DisplayMessage([string.Format("\nParked: {0} at parking spot {1}", vehicle, index + 1)]);
            }


            string CreateVehicle(string vehicleType, string reg)
            {
                return string.Join("#", [vehicleType, reg]);
            }


            void AddVehicle(string vehicle, int index)
            {
                string[] split = vehicle.Split('#');

                string vehicleType = split[0];
                string reg = split[1];

                if (string.IsNullOrEmpty(parkingGarage[index]))
                {
                    parkingGarage[index] = vehicle;
                    return;
                }

                //if there is a parked vehicle and its type is MC check if there is space for another
                if (vehicleType == "MC")
                {
                    string[] s = parkingGarage[index].Split('/');

                    if (s.Length < 2)
                    {
                        parkingGarage[index] = string.Join('/', [s[0], vehicle]);
                    }
                }
            }


            void FindVehicle()
            {
                string reg = DisplayMenu(["Type the registration number of your vehicle.\n", "Reg number example: ABC123\n"]);
                reg = InputValidation.ValidateRegNumber(reg);

                int vehicleIndex = SearchForVehicle(reg);

                if (vehicleIndex == -1)
                {
                    DisplayMessage([string.Format("\n{0} does not exist in the parking lot.", reg)]);
                    return;
                }

                ManageVehicle(reg, vehicleIndex);
            }


            void ManageVehicle(string reg, int index)
            {
                input = DisplayMenu(["What would you like to do with the vehicle?\n", "[1]: Remove vehicle", "[2]: Move vehicle\n"]);

                while (input != "1" && input != "2")
                {
                    Console.WriteLine("{0} is not a valid choice, try again.", input);
                    input = InputValidation.EvaluateInputString();
                }

                if (input == "1")
                {
                    string vehicle = RemoveVehicle(reg, index);
                    DisplayMessage([string.Format("\nRemoving vehicle: {0} at parking space {1}", vehicle, index + 1)]);
                }
                else if (input == "2")
                {
                    MoveVehicle(reg, index);
                }
            }


            //removes a vehicle from the parking garage and returns it in case it should be moved
            string RemoveVehicle(string reg, int index)
            {
                string[] vehicles = parkingGarage[index].Split('/');

                string vehicleToKeep = "";
                string vehicleToRemove = "";

                for (int i = 0; i < vehicles.Length; i++)
                {
                    string[] split = vehicles[i].Split('#');

                    if (split[1] == reg)
                    {
                        vehicleToRemove = vehicles[i];
                    }
                    else
                    {
                        vehicleToKeep = vehicles[i];
                    }
                }

                parkingGarage[index] = vehicleToKeep;
                return vehicleToRemove;
            }


            void MoveVehicle(string reg, int moveIndex)
            {
                input = DisplayMenu(["Where would you like to move the vehicle?", "Type a number between 1-100 to choose a parking space\n"]);
                int inputIndex = InputValidation.ValidateIndex(parkingGarage, input);

                string vehicle = RemoveVehicle(reg, moveIndex);
                string[] split = vehicle.Split('#');

                inputIndex = FindVacantParkingSpace(split[0], inputIndex);
                AddVehicle(vehicle, inputIndex);

                DisplayMessage([string.Format("\nMoved: {0} from parking spot {1} to parking spot {2}", vehicle, moveIndex + 1, inputIndex + 1)]);
            }


            void DisplayParkingSpace()
            {
                string input = DisplayMenu(["Type a number between 1-100 to choose a parking space to display.\n"]);
                int inputNumber = InputValidation.ValidateIndex(parkingGarage, input);

                DisplayMessage([string.Format("\n{0}: {1}", inputNumber + 1, parkingGarage[inputNumber])]);
            }


            void DisplayParkingLot()
            {
                Console.Clear();

                for (int i = 0; i < parkingGarage.Length; i++)
                {
                    //only display parking spaces with vehicles
                    if (string.IsNullOrEmpty(parkingGarage[i]))
                    {
                        continue;
                    }

                    Console.WriteLine($"{i + 1}: {parkingGarage[i]}");
                }

                Console.WriteLine("\nPress enter to return to main menu.\n");
                Console.ReadKey();
            }


            //returns index of parked vehicle
            int SearchForVehicle(string reg)
            {
                for (int i = 0; i < parkingGarage.Length; i++)
                {
                    if (string.IsNullOrEmpty(parkingGarage[i]))
                    {
                        continue;
                    }

                    string[] split = parkingGarage[i].Split('#', '/');
                    foreach (string s in split)
                    {
                        if (s.Contains(reg))
                        {
                            return i;
                        }
                    }
                }

                return -1;
            }


            //search for vacancy on a single parking space
            int FindVacantParkingSpace(string type, int index)
            {
                string input;

                while (string.IsNullOrEmpty(parkingGarage[index]) == false)
                {
                    //if the vehicle is a MC some checks have to be done to see if there is a vacancy in the parking space
                    if (type == "MC")
                    {
                        string[] split = parkingGarage[index].Split('#', '/');

                        if (split.Contains(type) && split.Length < 4)
                        {
                            break;
                        }
                    }

                    Console.WriteLine("\nParking space {0} is not vacant, try again.\n", index + 1);
                    input = InputValidation.EvaluateInputString();
                    index = InputValidation.ValidateIndex(parkingGarage, input);
                }

                return index;
            }
        }
    }

    public static class InputValidation
    {
        //makes sure that the input string is not empty
        public static string EvaluateInputString()
        {
            string? input = Console.ReadLine();

            while (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("\nInvalid input, try again.\n");

                input = Console.ReadLine();
            }

            return input.ToUpper();
        }


        //checks that the input string is a number or asks the user to try again
        public static int EvaluateInt(string input)
        {
            bool result = int.TryParse(input, out int number);

            while (result == false)
            {
                Console.WriteLine("\n{0} is not a number, try again.\n", input);

                input = EvaluateInputString();
                result = int.TryParse(input, out number);
            }

            return number;
        }


        public static int ValidateIndex(Array arr, string input)
        {
            int index = EvaluateInt(input);
            index--;

            while (index > arr.Length - 1 || index < 0)
            {
                Console.WriteLine("\n{0} is not a valid parking space, try again.\n", index + 1);

                input = EvaluateInputString();
                index = EvaluateInt(input);
                index--;
            }

            return index;
        }


        public static string ValidateRegNumber(string reg)
        {
            string pattern = @"^[A-Z]{3}[0-9]{3}$";
            Regex regex = new Regex(pattern);

            while (regex.IsMatch(reg) == false)
            {
                Console.WriteLine("\n{0} is not a valid reg number, try again.\n", reg);
                reg = EvaluateInputString();
            }

            return reg;
        }


        public static string ValidateVehicleType(string type)
        {
            while (type != "1" && type != "2")
            {
                Console.WriteLine("\n{0} is not a vehicle type, try again.\n", type);
                type = EvaluateInputString();
            }

            if (type == "1")
            {
                return "CAR";
            }

            return "MC";
        }
    }
}