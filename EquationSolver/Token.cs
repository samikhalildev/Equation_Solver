using System;

namespace Equation_Solver {
    
    public class Token {
        
        private string value;
        private int precedence;

        public string Value { get => value; set => this.value = value; }
        public int Precedence { get => precedence; set => precedence = value; }

        public Token(string value){
            this.Value = value;
        }

        public bool isNumber(int n) {
            return (int.TryParse(Value, out n));
        }

        public bool isVariable(){
            return value.Equals("X");
        }

        public bool isEquals(){
            return value.Equals("=");
        }

        public bool isAnOperator(){
            return ((Value.Equals("+") || Value.Equals("-") || Value.Equals("*") || Value.Equals("/") || Value.Equals("%")));
        }

        public void setPrecedence(){

            switch(Value){
                case "+":
                case "-":
                    Precedence = 1;
                    break;

                case "*":
                case "/":
                case "%":
                    Precedence = 2;
                    break;

            }
        }
    }
}
