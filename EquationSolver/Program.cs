using System;

namespace Equation_Solver {

    class MainClass {

        public static void Main(string[] args){

            Equation equation = new Equation(args); 
            equation.PrintX();

        }
    }
}
