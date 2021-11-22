using System;
using Npgsql;
using System.Collections.Generic;
using static System.IO.File;
using System.IO;

namespace code
{
    class Program
    {
        static NpgsqlConnection connection;
        static void Main(string[] args)
        {
            string connString = "Host=localhost;Port=5433;Database=TrafficOrganization;Username=postgres;Password=1a2s3d4f";
            connection = new NpgsqlConnection(connString);
            connection.Open();
            RandomCar();
            OutputCars(TakeCars(0));
            connection.Close();

            // Random random = new Random();
            // DateTime d = new DateTime( random.Next(2015, 2022), random.Next(1,13), random.Next(1,32), random.Next(0,24), random.Next(0,60), random.Next(0,60));
            // Street street = new Street();
            //Console.WriteLine(RandomCarNumber());

            // PIB pib = new PIB();
            // string[] fullname = pib.RandomPIB();
            // Console.WriteLine($"{fullname[0]} {fullname[1]} {fullname[2]}");
        }
        static void RandomCamera()
        {
            Street street = new Street();
            InsertCamera(NextCId(), street.RandomStreet());
        }
        static void RandomCar()
        {
            Random random = new Random();
            try
            {
                using (var command = new NpgsqlCommand("INSERT INTO \"cars\" (id, carnumber, ownerid, cameraid, speed, timeofregistrationc) VALUES (@id, @carnumber, @ownerid, @cameraid, floor(random() * (200-10+1) + 10)::int, @timeofregistrationc)", connection))
                {
                    command.Parameters.AddWithValue("id", NextCarId());
                    command.Parameters.AddWithValue("carnumber", RandomCarNumber()); //TODO check
                    command.Parameters.AddWithValue("ownerid", RandomCarOwner());
                    command.Parameters.AddWithValue("cameraid", random.Next(1, NextCId()));
                    command.Parameters.AddWithValue("timeofregistrationc", new DateTime( random.Next(2015, 2022), random.Next(1,13), random.Next(1,32), random.Next(0,24), random.Next(0,60), random.Next(0,60))); //???????
                    command.ExecuteNonQuery();
                }
            }
            catch(NpgsqlException ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }
        static int RandomCarOwner()
        {
            PIB pib = new PIB();
            int id = NextOId();
            InsertCarOwner(id, pib.RandomPIB());
            return id;
        }
        static string RandomCarNumber()
        {
            string[] letters = new string[12]{"A", "B", "C", "E", "H", "I", "K", "M", "O", "P", "T", "X"};
            string carnum = "";
            Random random = new Random();
            carnum += letters[random.Next(0,12)];
            carnum += letters[random.Next(0,12)];
            carnum += random.Next(1000,10000).ToString();
            carnum += letters[random.Next(0,12)];
            carnum += letters[random.Next(0,12)];
            return carnum;
        }

        #region car
        static void OutputCars(List<string[]> list)
        {
            foreach (string[] item in list)
            {
                string fullname = TakeFullname(int.Parse(item[2]));
                string street = TakeStreet(int.Parse(item[3]));
                Console.WriteLine($"{item[0]}. {item[1]}\n\towner: {fullname}\n\tstreet: ({item[3]}){street}\n\tspeed: {item[4]} km/h\n\ttime: {item[5]}");
            }
        }
        static List<string[]> TakeCars(int cameraId)
        {
            List<string[]> listOfCars = new List<string[]>();
            using (var command = new NpgsqlCommand("SELECT * FROM \"cars\"", connection))
            {
                var reader = command.ExecuteReader();
                
                while(reader.Read())
                {
                    if(cameraId == 0 || cameraId == (int)reader[3])
                    {
                        string[] car = new string[6];
                        car[0] = reader[0].ToString(); //id
                        car[1] = reader[1].ToString(); //carnumber
                        car[2] = reader[2].ToString(); //ownerid
                        car[3] = reader[3].ToString(); //cameraid
                        car[4] = reader[4].ToString(); //speed
                        car[5] = reader[5].ToString(); //timeofregistrationc
                        listOfCars.Add(car);
                    }
                }
                reader.Close();
            }
            return listOfCars;
        }
        static void InsertCar(int id, string carnumber, int ownerid, int cameraid, int speed, DateTime timeofregistrationc)
        {
            try
            {
                using (var command = new NpgsqlCommand("INSERT INTO \"cars\" (id, carnumber, ownerid, cameraid, speed, timeofregistrationc) VALUES (@id, @carnumber, @ownerid, @cameraid, @speed, @timeofregistrationc)", connection))
                {
                    command.Parameters.AddWithValue("id", id);
                    command.Parameters.AddWithValue("carnumber", carnumber); //TODO check
                    command.Parameters.AddWithValue("ownerid", ownerid);
                    command.Parameters.AddWithValue("cameraid", cameraid);
                    command.Parameters.AddWithValue("speed", speed);
                    command.Parameters.AddWithValue("timeofregistrationc", timeofregistrationc); //???????
                    command.ExecuteNonQuery();
                }
            }
            catch(NpgsqlException ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }
        static bool CheckCarNumber(string carnumber)
        {
            char[] charArr = carnumber.ToCharArray();
            for(int i = 0; i < charArr.Length; i++)
            {
                if(i < 2 || i > 5)
                {
                    if(charArr[i] < 65 || charArr[i] > 90)
                    {
                        return false;
                    }
                }
                if(i >= 2 && i <= 5)
                {
                    if(charArr[i] < 48 || charArr[i] > 57)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        static int NextCarId()
        {
            Int32 max = 0;
            using (var command = new NpgsqlCommand("SELECT * FROM \"cars\"", connection))
            {
                var reader = command.ExecuteReader();
                
                while(reader.Read())
                {
                    Int32 num = (Int32) reader[0];
                    if(num >= max)
                    {
                        max = num;
                    }
                }
                reader.Close();
            }
            return max + 1;
        }
        #endregion
        #region carowner
        static void InsertCarOwner(int id, string fullname)
        {
            try
            {
                using (var command = new NpgsqlCommand("INSERT INTO \"carowners\" (id, fullname) VALUES (@id, @fullname)", connection))
                {
                    command.Parameters.AddWithValue("id", id);
                    command.Parameters.AddWithValue("fullname", fullname);
                    command.ExecuteNonQuery();
                }
            }
            catch(NpgsqlException ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }
        static void TakeCarOwners()
        {
            using (var command = new NpgsqlCommand("SELECT * FROM \"carowners\"", connection))
            {
                List<string[]> listOfCarOwners = new List<string[]>();
                var reader = command.ExecuteReader();
                
                while(reader.Read())
                {
                    string[] carOwners = new string[2];
                    carOwners[0] = reader[0].ToString(); //id
                    carOwners[1] = reader[1].ToString(); //fullname
                    listOfCarOwners.Add(carOwners);
                }
                reader.Close();
                //OutputCar(listOfCarOwners);
            }
        }
        static string TakeFullname(int id)
        {
            string fullname = "";
            using (var command = new NpgsqlCommand("SELECT * FROM \"carowners\"", connection))
            {
                var reader = command.ExecuteReader();
                
                while(reader.Read())
                {
                    int currentId = (int)reader[0];
                    if(currentId == id)
                    {
                        fullname = reader[1].ToString();
                    }
                }
                reader.Close();
            }
            return fullname;
        }
        static int NextOId()
        {
            Int32 max = 0;
            using (var command = new NpgsqlCommand("SELECT * FROM \"carowners\"", connection))
            {
                var reader = command.ExecuteReader();
                
                while(reader.Read())
                {
                    Int32 num = (Int32) reader[0];
                    if(num >= max)
                    {
                        max = num;
                    }
                }
                reader.Close();
            }
            return max + 1;
        }
        #endregion
        #region camera
        static void InsertCamera(int id, string locationaddress)
        {
            try
            {
                using (var command = new NpgsqlCommand("INSERT INTO \"cameras\" (id, locationaddress) VALUES (@id, @locationaddress)", connection))
                {
                    command.Parameters.AddWithValue("id", id);
                    command.Parameters.AddWithValue("locationaddress", locationaddress);
                    command.ExecuteNonQuery();
                }
            }
            catch(NpgsqlException ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }
        static string TakeStreet(int id)
        {
            string street = "";
            using (var command = new NpgsqlCommand("SELECT * FROM \"cameras\"", connection))
            {
                var reader = command.ExecuteReader();
                
                while(reader.Read())
                {
                    int currentId = (int)reader[0];
                    if(currentId == id)
                    {
                        street = reader[1].ToString();
                    }
                }
                reader.Close();
            }
            return street;
        }
        static int NextCId()
        {
            Int32 max = 0;
            using (var command = new NpgsqlCommand("SELECT * FROM \"cameras\"", connection))
            {
                var reader = command.ExecuteReader();
                
                while(reader.Read())
                {
                    Int32 num = (Int32) reader[0];
                    if(num >= max)
                    {
                        max = num;
                    }
                }
                reader.Close();
            }
            return max + 1;
        }
        #endregion
    }
    class Street
    {
        string filePath = "./streets.csv";
        string[] streets;
        public Street()
        {
            StreamReader sr = new StreamReader(filePath);
            string text = ReadAllText(filePath);
            streets = text.Split("\r\n");
        }
        public string RandomStreet()
        {
            Random random = new Random();
            int i = random.Next(0, streets.Length);
            return streets[i];
        }
    }
    class PIB
    {
        string file_surname = "./surname.csv";
        string file_name = "./names.csv";
        string file_middleName = "./middle names.csv";
        string[] surnames;
        string[] femaleName;
        string[] maleName;
        string[] femaleMiddleName;
        string[] maleMiddleName;
        public PIB()
        {
            ListOfSurname();
            ListsOfNames();
            ListsOfMiddleNames();
        }
        private void ListOfSurname()
        {
            StreamReader sr = new StreamReader(file_surname);
            string text = ReadAllText(file_surname);
            surnames = text.Split("\r\n");
        }
        private void ListsOfNames()
        {
            List<string> fName = new List<string>();
            List<string> mName = new List<string>();
            StreamReader sr = new StreamReader(file_name);
            string text = ReadAllText(file_name);
            string[] str = text.Split("\r\n");
            foreach (var item in str)
            {
                string[] names = item.Split(",");
                fName.Add(names[0]);
                mName.Add(names[1]);
            }
            femaleName = new string[fName.Count];
            int i = 0;
            foreach (var item in fName)
            {
                femaleName[i] = item;
                i++;
            }
            maleName = new string[mName.Count];
            i = 0;
            foreach (var item in mName)
            {
                maleName[i] = item;
                i++;
            }
        }
        private void ListsOfMiddleNames()
        {
            List<string> fMName = new List<string>();
            List<string> mMName = new List<string>();
            StreamReader sr = new StreamReader(file_middleName);
            string text = ReadAllText(file_middleName);
            string[] str = text.Split("\r\n");
            foreach (var item in str)
            {
                string[] names = item.Split(",");
                fMName.Add(names[0]);
                mMName.Add(names[1]);
            }
            femaleMiddleName = new string[fMName.Count];
            int i = 0;
            foreach (var item in fMName)
            {
                femaleMiddleName[i] = item;
                i++;
            }
            maleMiddleName = new string[mMName.Count];
            i = 0;
            foreach (var item in mMName)
            {
                maleMiddleName[i] = item;
                i++;
            }
        }
        public string RandomPIB()
        {
            string PIB = "";
            Random random = new Random();
            int i = random.Next(0, surnames.Length);
            PIB += surnames[i] + " ";
            i = random.Next(0, 2);
            if(i == 0) //male
            {
                i = random.Next(0, maleName.Length);
                PIB += maleName[i] + " ";
                i = random.Next(0, maleMiddleName.Length);
                PIB += maleMiddleName[i];
            }
            else //female
            {
                i = random.Next(0, femaleName.Length);
                PIB += femaleName[i] + " ";
                i = random.Next(0, femaleMiddleName.Length);
                PIB += femaleMiddleName[i];
            }
            return PIB;
        }
    }
}
