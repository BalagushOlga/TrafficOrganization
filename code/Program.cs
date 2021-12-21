using System;
using Npgsql;
using System.Collections.Generic;
using static System.IO.File;
using System.IO;

namespace code
{
    class Program
    {
        static void Main(string[] args)
        {
            View view = new View();
            Model model = new Model();
            Graphic graphic = new Graphic(model);
            Index index = new Index(model);
            Exute exute = new Exute();
            Controller controller = new Controller(view, model, graphic, index, exute);
            string command = "";
            while(command != "exit")
            {
                command = view.Command();
                controller.CommadLine(command);
            }
        }
    }
}