using System;
using Npgsql;
using System.Collections.Generic;
using static System.IO.File;
using System.IO;
using System.Diagnostics;

public class Model
{
    private NpgsqlConnection connection;
    public Model()
    {
        string connString = "Host=localhost;Port=5433;Database=TrafficOrganization;Username=postgres;Password=1a2s3d4f";
        connection = new NpgsqlConnection(connString);
    }
    public NpgsqlConnection Connection()
    {
        return this.connection;
    }
    public void RandomCamera()
    {
        Street street = new Street();
        InsertCamera(NextCId(), street.RandomStreet());
    }
    public void RandomCar()
    {
        int carId =  NextCarId();
        int ownerId = RandomCarOwner();
        Random random = new Random();
        int cameraId = random.Next(1, NextCId());
        connection.Open();
        try
        {
            using (var command = new NpgsqlCommand("INSERT INTO \"cars\" (id, carnumber, ownerid, cameraid, speed, timeofregistrationc) VALUES (@id, @carnumber, @ownerid, @cameraid, floor(random() * (200-10+1) + 10)::int, @timeofregistrationc)", connection))
            {
                command.Parameters.AddWithValue("id",carId);
                command.Parameters.AddWithValue("carnumber", RandomCarNumber());
                command.Parameters.AddWithValue("ownerid", ownerId);
                command.Parameters.AddWithValue("cameraid", cameraId);
                command.Parameters.AddWithValue("timeofregistrationc", new DateTime( random.Next(2000, 2021), random.Next(1,12), random.Next(1,28), random.Next(0,24), random.Next(0,60), random.Next(0,60))); //???????
                command.ExecuteNonQuery();
            }
        }
        catch(NpgsqlException ex)
        {
            Console.WriteLine($"{ex.Message}");
        }
        connection.Close();
    }
    public int RandomCarOwner()
    {
        PIB pib = new PIB();
        int id = NextOId();
        InsertCarOwner(id, pib.RandomPIB());
        return id;
    }
    public string RandomCarNumber()
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
    public List<string[]> OutputCars(List<string[]> list)
    {
        foreach (string[] item in list)
        {
            item[2] = TakeFullname(int.Parse(item[2]));
            item[3] = TakeStreet(int.Parse(item[3]));
        }
        return list;
    }
    public List<string[]> TakeCars(int cameraId)
    {
        connection.Open();
        List<string[]> listOfCars = new List<string[]>();
        using (var command = new NpgsqlCommand("SELECT * FROM \"cars\" WHERE cameraid = @cameraid", connection))
        {
            var reader = command.ExecuteReader();
            
            while(reader.Read())
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
            reader.Close();
        }
        connection.Close();
        return listOfCars;
    }
    public void InsertCar(int id, string carnumber, int ownerid, int cameraid, int speed, DateTime timeofregistrationc)
    {
        if(id == -1)
        {
            id = NextCarId();
        }
        if(!CheckCarNumber(carnumber))
        {
            return;
        }
        connection.Open();
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
        connection.Close();
    }
    public void UpdateCar(int id, string carnumber, int ownerid, int cameraid, int speed, DateTime timeofregistrationc)
    {
        if(!CheckCarNumber(carnumber))
        {
            return;
        }
        connection.Open();
        try
        {
            using (var command = new NpgsqlCommand("UPDATE \"cars\" SET carnumber = @carnumber, ownerid = @ownerid, cameraid = @cameraid, speed = @speed, timeofregistrationc = @timeofregistrationc WHERE id = @id", connection))
            {
                command.Parameters.AddWithValue("id", id);
                command.Parameters.AddWithValue("carnumber", carnumber); //TODO check
                command.Parameters.AddWithValue("ownerid", ownerid);
                command.Parameters.AddWithValue("cameraid", cameraid);
                command.Parameters.AddWithValue("speed", speed);
                command.Parameters.AddWithValue("timeofregistrationc", timeofregistrationc);
                command.ExecuteNonQuery();
            }
        }
        catch(NpgsqlException ex)
        {
            Console.WriteLine($"{ex.Message}");
        }
        connection.Close();
    }
    public void DeleteCar(int id)
    {
        connection.Open();
        try
        {
            int ownerId = -1;
            using (var command = new NpgsqlCommand("SELECT * FROM \"cars\" WHERE id = @id", connection))
            {
                command.Parameters.AddWithValue("id", id);
                var reader = command.ExecuteReader();
                ownerId = (int)reader[2];
                reader.Close();
            }
            using (var command = new NpgsqlCommand("DELETE FROM \"cars\" WHERE id = @id", connection))
            {
                command.Parameters.AddWithValue("id", id);
                command.ExecuteNonQuery();
            }
            List<int> listOfCars = ListOfCar(ownerId);
            if(listOfCars.Count == 1)
            {
                DeleteCarOwner(ownerId);
            }
        }
        catch(NpgsqlException ex)
        {
            Console.WriteLine($"{ex.Message}");
        }
        connection.Close();
    }
    public bool CheckCarNumber(string carnumber)
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
    public int NumberOfCarWithSpeed(int minSpeed, int maxSpeed)
    {
        int numberOfCars = 0;
        connection.Open();
        try
        {
            using (var command = new NpgsqlCommand("SELECT * FROM \"cars\"", connection))
            {
                var reader = command.ExecuteReader();
                while(reader.Read())
                {
                    if((int)reader[4] >= minSpeed && (int)reader[4] <= maxSpeed)
                    {
                        numberOfCars++;
                    }
                }
                reader.Close();
            }
        }
        catch(NpgsqlException ex)
        {
            Console.WriteLine($"{ex.Message}");
        }
        connection.Close();
        return numberOfCars;
    }
    public Int64 NumberOfCars()
    {
        //SELECT COUNT(*) FROM cars
        Int64 numberOfCars = 0;
        connection.Open();
        try
        {
            using (var command = new NpgsqlCommand("SELECT COUNT(*) FROM \"cars\"", connection))
            {
                numberOfCars = (Int64)command.ExecuteScalar();
            }
        }
        catch(NpgsqlException ex)
        {
            Console.WriteLine($"{ex.Message}");
        }
        connection.Close();
        return numberOfCars;
    }
    public Int64 TakeNumberOfCarsOwner(int ownerId)
    {
        Int64 numberOfCars = 0;
        connection.Open();
        try
        {
            using (var command = new NpgsqlCommand("SELECT COUNT(*) FROM \"cars\" WHERE ownerid = @ownerid", connection))
            {
                command.Parameters.AddWithValue("ownerid", ownerId);
                numberOfCars = (Int64)command.ExecuteScalar();
            }
        }
        catch(NpgsqlException ex)
        {
            Console.WriteLine($"{ex.Message}");
        }
        connection.Close();
        return numberOfCars;
    }
    private int NextCarId()
    {
        connection.Open();
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
        connection.Close();
        return max + 1;
    }
    #endregion
    #region carowner
    public void InsertCarOwner(int id, string fullname)
    {
        if(id == -1)
        {
            id = NextOId();
        }
        connection.Open();
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
        connection.Close();
    }
    public void UpdateCarOwner(int id, string fullname)
    {
        connection.Open();
        try
        {
            using (var command = new NpgsqlCommand("UPDATE \"carowners\" SET fullname = @fullname WHERE id = @id", connection))
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
        connection.Close();
    }
    public void DeleteCarOwner(int id)
    {
        connection.Open();
        try
        {
            using (var command = new NpgsqlCommand("DELETE FROM \"carowners\" WHERE id = @id", connection))
            {
                command.Parameters.AddWithValue("id", id);
                command.ExecuteNonQuery();
            }
        }
        catch(NpgsqlException ex)
        {
            Console.WriteLine($"{ex.Message}");
        }
        connection.Close();
    }
    public List<int> ListOfCar(int ownerId)
    {
        List<int> listOfCars = new List<int>();
        connection.Open();
        try
        {
            using (var command = new NpgsqlCommand("SELECT * FROM \"cars\" WHERE ownerid = @ownerid", connection))
            {
                command.Parameters.AddWithValue("ownerid", ownerId);
                var reader = command.ExecuteReader();
                while(reader.Read())
                {
                    listOfCars.Add((int)reader[0]);
                }
                reader.Close();
            }
        }
        catch(NpgsqlException ex)
        {
            Console.WriteLine($"{ex.Message}");
        }
        connection.Close();
        return listOfCars;
    }
    public List<string[]> TakeCarOwners()
    {
        List<string[]> listOfCarOwners = new List<string[]>();
        connection.Open();
        try
        {
            using (var command = new NpgsqlCommand("SELECT * FROM \"carowners\"", connection))
            {
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
        catch(NpgsqlException ex)
        {
            Console.WriteLine($"{ex.Message}");
        }
        connection.Close();
        return listOfCarOwners;
    }
    public string TakeFullname(int id)
    {
        connection.Open();
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
        connection.Close();
        return fullname;
    }
    private int NextOId()
    {
        connection.Open();
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
        connection.Close();
        return max + 1;
    }
    #endregion
    #region camera
    public void InsertCamera(int id, string locationaddress)
    {
        if(id == -1)
        {
            id = NextCId();
        }
        connection.Open();
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
        connection.Close();
    }
    public void UpdateCamera(int id, string locationaddress)
    {
        connection.Open();
        try
        {
            using (var command = new NpgsqlCommand("UPDATE \"cameras\" SET locationaddress = @locationaddress WHERE id = @id", connection))
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
        connection.Close();
    }
    public void DeleteCamera(int id)
    {
        // мб позначати видаленою, але не видаляти??
        connection.Open();
        try
        {
            using (var command = new NpgsqlCommand("DELETE FROM \"cameras\" WHERE id = @id", connection))
            {
                command.Parameters.AddWithValue("id", id);
                command.ExecuteNonQuery();
            }
        }
        catch(NpgsqlException ex)
        {
            Console.WriteLine($"{ex.Message}");
        }
        connection.Close();
    }
    public string TakeStreet(int id)
    {
        connection.Open();
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
        connection.Close();
        return street;
    }
    public List<string[]> AllStreets()
    {
        List<string[]> streetsIdAndName = new List<string[]>();
        connection.Open();
        try
        {
            using (var command = new NpgsqlCommand("SELECT * FROM \"cameras\"", connection))
            {
                var reader = command.ExecuteReader();
                
                while(reader.Read())
                {
                    string[] street = new string[2];
                    street[0] = reader[0].ToString(); //id
                    street[1] = reader[1].ToString();
                    streetsIdAndName.Add(street);
                }
                reader.Close();
            }
        }
        catch(NpgsqlException ex)
        {
            Console.WriteLine($"{ex.Message}");
        }
        connection.Close();
        return streetsIdAndName;
    }
    private int NextCId()
    {
        connection.Open();
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
        connection.Close();
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
class Graphic
{
    Model model;
    public Graphic(Model model)
    {
        this.model = model;
    }
    private List<double> Percentages(Int64 allEntities, List<int> listOfEntities)
    {
        List<double> list = new List<double>();
        foreach (var item in listOfEntities)
        {
            double procent = item * 100 / allEntities;
            list.Add(procent);
        }
        return list;
    }
    public string SpeedPercentage()
    {
        var plt = new ScottPlot.Plot(600, 400);

        Int64 allEntities = model.NumberOfCars();
        List<int> listOfEntities = new List<int>(){model.NumberOfCarWithSpeed(0, 50), model.NumberOfCarWithSpeed(51, 70), model.NumberOfCarWithSpeed(71, 300)};
        double[] values = new double[3];
        int i = 0;
        if(Percentages(allEntities, listOfEntities).Count == 3)
        {
            foreach (var item in Percentages(allEntities, listOfEntities))
            {
                values[i] = item;
                i++;
            }
        }
        string[] labels = { "0-50 km/h", "50-70 km/h", "from 70 km/h"};

        plt.PlotPie(values, labels, showPercentages: true, showValues: true, showLabels: true, explodedChart: true);
        plt.Legend();

        plt.Grid(false);
        plt.Frame(false);

        string file = "speed_chart.png";
        plt.SaveFig(file);
        return file;
    }
    private List<Int64[]> listCarsOwners()
    {
        Int64[] listNumberOfCars = new Int64[model.TakeCarOwners().Count];
        int k = 0;
        foreach (var item in model.TakeCarOwners())
        {
            int ownerId = int.Parse(item[0]);
            listNumberOfCars[k] = model.TakeNumberOfCarsOwner(ownerId);
            k++;
        }
        List<Int64[]> list = new List<Int64[]>();
        for(int i = 0; i < listNumberOfCars.Length; i++)
        {
            bool exist = false;
            foreach (var item in list)
            {
                if(item[0] == listNumberOfCars[i])
                {
                    exist = true;
                }
            }
            if(exist == false)
            {
                Int64[] c = new Int64[2];
                int count = 0;
                for(int j = i; j < listNumberOfCars.Length; j++)
                {
                    if(listNumberOfCars[j] == listNumberOfCars[i])
                    {
                        count++;
                    }
                }
                c[0] = listNumberOfCars[i];
                c[1] = count;
                list.Add(c);
            }
        }
        return list;
    }
    public string NumberOfCars()
    {
        //скільки власників мають певну кількість машин
        var plt = new ScottPlot.Plot(600, 400);

        List<Int64[]> list = listCarsOwners();

        int pointCount = list.Count;
        double[] ys = new double[pointCount];
        int k = 0;
        foreach (var item in list)
        {
            ys[k] = item[1];
            k++;
        }
        double[] xs = new double[pointCount];
        string[] labels = new string[pointCount];
        k = 0;
        foreach (var item in list)
        {
            xs[k] = item[0];
            labels[k] = item[0].ToString() + "car(s)";
            k++; 
        }
        
        plt.PlotBar(xs, ys, horizontal: true);
        plt.YTicks(xs, labels);

        string file = "numbers_of_car_owners.png";
        plt.SaveFig(file);
        return file;
    }
    public string CarsOnStreet()
    {
        // відсоток машин на вулицях
        int points = model.AllStreets().Count;

        var plt = new ScottPlot.Plot(600, 400);

        double[] values = new double[points];
        string[] labels = new string[points];
        int i = 0;
        foreach (var item in model.AllStreets())
        {
            int streetId = int.Parse(item[0]);
            values[i] = model.TakeCars(streetId).Count;
            labels[i] = item[1];
            i++;
        }

        plt.PlotPie(values, labels , showPercentages: true, showValues: true, showLabels: true);
        plt.Legend();

        plt.Grid(false);
        plt.Frame(false);
        string file = "street_statistic.png";
        plt.SaveFig(file);
        return file;
    }
}
class Index
{
    private Model model;
    public Index(Model model)
    {
        this.model = model;
    }
    public long[] _Index()
    {
        var result = model.TakeCars(1);

        Stopwatch sw = new Stopwatch();
        sw.Start();
        result = model.TakeCars(1);
        sw.Stop();

        long[] list = new long[2];
        list[0] =  sw.ElapsedMilliseconds;

        model.Connection().Open();
        using (var command = new NpgsqlCommand("CREATE INDEX index\n" + "ON \"cars\" USING gin (\"cameraid\");", model.Connection())){}
        model.Connection().Close();

        sw = new Stopwatch();
        sw.Start();
        result = model.TakeCars(1);
        sw.Stop();

        list[1] =  sw.ElapsedMilliseconds;

        model.Connection().Open();
        using (var command = new NpgsqlCommand("DROP INDEX index", model.Connection())){}
        model.Connection().Close();

        return list;
    }
}
class Exute
{
    public void BackUp()
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "C:/Users/Olha/Desktop/TrafficOrganization/code/backup.bat",
                Arguments = $"{DateTime.Now.ToFileTime()}",
                UseShellExecute = false,
                CreateNoWindow = true,
                Verb = "runas"
            }
        };

        process.Start();
        process.WaitForExit();
    }
    public void Restore(string fileName)
    {
        if (!File.Exists(fileName))
        {
            return;
        }
        if (!fileName.EndsWith(".sql"))
        {
            Console.WriteLine("You should choose sql-file");
            return;
        }

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "C:/Users/Olha/Desktop/TrafficOrganization/code/restore.bat",
                Arguments = "C:/Users/Olha/Desktop/TrafficOrganization/code/backup/" + fileName,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };
        process.Start();
        process.WaitForExit();
    }
}