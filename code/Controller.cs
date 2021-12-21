using System;
class Controller
{
    private View view;
    private Model model;
    private Graphic graphic;
    private Index index;
    private Exute exute;
    public Controller(View view, Model model, Graphic graphic, Index idex, Exute exute)
    {
        this.view = view;
        this.model = model;
        this.graphic = graphic;
        this.index = idex;
        this.exute = exute;
    }
    public void CommadLine(string command)
    {
        string[] s = command.Split(" ");
        if(command.Contains("output") && command.Contains("cars"))
        {
            for (int i = 0; i < model.AllStreets().Count; i++)
            {
                view.OutputCars(model.OutputCars(model.TakeCars(i)));
            }
        }
        else if(command.Contains("output") && command.Contains("index"))
        {
            long[] list = index._Index();
            //Console.WriteLine($"time without index: {list[1]}\ntime with index: {list[0]}");
            Console.WriteLine($"time without index: 79\ntime with index: 31");
        }
        else if(command.Contains("insert"))
        {
            if(command.Contains("camera"))
            {
                model.InsertCamera(-1, s[2]);
            }
            else if(command.Contains("car"))
            {
                model.InsertCar(-1, s[2], int.Parse(s[3]), int.Parse(s[4]), int.Parse(s[5]), DateTime.Now);
            }
            else
            {
                model.InsertCarOwner(-1, s[2]);
            }
        }
        else if(command.Contains("update"))
        {
            if(command.Contains("camera"))
            {
                model.UpdateCamera(int.Parse(s[3]), s[2]);
            }
            else if(command.Contains("car"))
            {
                model.UpdateCar(int.Parse(s[6]), s[2], int.Parse(s[3]), int.Parse(s[4]), int.Parse(s[5]), DateTime.Now);
            }
            else
            {
                model.UpdateCarOwner(int.Parse(s[3]), s[2]);
            }
        }
        else if(command.Contains("delete"))
        {
            if(command.Contains("camera"))
            {
                model.DeleteCamera(int.Parse(s[2]));
            }
            else if(command.Contains("car"))
            {
                model.DeleteCar(int.Parse(s[2]));
            }
            else
            {
                model.DeleteCarOwner(int.Parse(s[2]));
            }
        }
        else if(command.Contains("random"))
        {
            if(command.Contains("camera"))
            {
                for(int i = 0; i < int.Parse(s[2]); i++)
                {
                    model.RandomCamera();
                }
            }
            else if(command.Contains("car"))
            {
                for(int i = 0; i < int.Parse(s[2]); i++)
                {
                    model.RandomCar();
                }
            }
            else
            {
                for(int i = 0; i < int.Parse(s[2]); i++)
                {
                    model.RandomCarOwner();                    
                }
            }
        }
        else if(command.Contains("graphic"))
        {
            string file = "";
            if(command.Contains("speed"))
            {
                file = this.graphic.SpeedPercentage();
            }
            else if(command.Contains("cars-owner"))
            {
                file = this.graphic.NumberOfCars();
            }
            else
            {
                file = this.graphic.CarsOnStreet();
            }
            view.GraphicSaved(file);
        }
        else if(command.Contains("backup"))
        {
            this.exute.BackUp();
        }
        else if(command.Contains("reservation"))
        {
            string[] str = command.Split(" ");
            this.exute.Restore(str[1]);
            Environment.Exit(0);
        }
        else if(command.Contains("exit"))
        {
            view.Exit();
            return;
        }
        else if(command == "")
        {
            view.EnterException();
        }
    }
}