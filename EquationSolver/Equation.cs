using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace Equation_Solver {

    public class Equation {

        // Properties
        List<Token> tokens = new List<Token>();
        Stack stack = new Stack();
        Queue queue = new Queue();

        Token PopToken, PeekToken, leftSide, rightSide, tokenResult;

        // Setup variables
        string[] equation;
        string operation = "";
        int left, right, n, result;
        char tempOperator = ' ';
        bool equalsFound, variableFound, XSolved, lookingForX = true;

        //Constructor
        public Equation(string[] equation) {
            this.equation = equation;
            SplitStr();
        }

        public void SplitStr() {

            try{
                // Iterate over each string in the equation
                foreach(string str in equation){
                    // If string is empty, skip it
                    if(str.Equals(" ") || str.Equals(",")){
                        continue;
                    }

                    // Convert "x" to "X"
                    if(str.Equals("x")){
                        tokens.Add(new Token(str.ToUpper()));
                        continue;
                    }

                    if(str.Contains("X") && str.Length > 1){
                        tokens.Add(new Token(str[0].ToString()));
                        tokens.Add(new Token("*"));
                        tokens.Add(new Token(str[1].ToString().ToUpper()));
                        continue;
                    }

                    // This will make sure if the same operator is entered multiple times, it will only add it once
                    if(str.Length > 1 && !int.TryParse(str, out n)){
                        foreach(char c in str){
                            if(tempOperator != ' ' || tempOperator == c)
                                break;
                            tempOperator = c;
                        }
                        tokens.Add(new Token(tempOperator.ToString()));
                        continue;
                    }
                    tokens.Add(new Token(str));
                }

            } catch (Exception e){
                Console.WriteLine("ERROR - " + e.Message);
            }
            InfixToPostfix();
        }

        // Convert Infix to Postfix notation (RPN)
        public void InfixToPostfix(){

            try{
                // Iterate over each token
                foreach(Token token in tokens){
                    // If it a number, add it to the queue
                    if (token.isNumber(n)){
                        queue.Enqueue(token);

                        // if token is X, add it to the queue and set variablefound to true
                    } else if (token.isVariable()){
                        queue.Enqueue(token);
                        variableFound = true;


                    } else if (token.isEquals()){
                        while(stack.Count != 0){
                            PopToken = Pop();
                            queue.Enqueue(PopToken);
                        }
                        equalsFound = true;
                        stack.Push(token);

                        // If it an operator
                    } else if (token.isAnOperator()){
                        token.setPrecedence();
                        // If there is an operator on top of the stack that is a higher or equal precedence than a current one or its not a left bracket
                        // Pop it out of the stack and add it to the queue
                        if(stack.Count != 0){
                            PeekToken = Peek();

                            while(stack.Count != 0 && HasHigherPrec(PeekToken.Precedence, token.Precedence)){
                                PopToken = Pop();
                                queue.Enqueue(PopToken);
                                PeekToken = Peek();
                            }
                        }

                        // Push the current operator onto the stack
                        stack.Push(token); 

                        // If it is a left bracket, push it to the stack
                    } else if (token.Value.Equals("(")){
                        stack.Push(token);

                        // If it is a right bracket (pop and add everything into the queue that is between the brackets)
                    } else if (stack.Count != 0 && token.Value.Equals(")")){
                        PopToken = Pop();
                        // While the operator at the top of the operator stack is not a left bracket
                        while(!PopToken.Value.Equals("(")){
                            queue.Enqueue(PopToken);
                            PopToken = Pop();
                        }
                    }
                }

                if(!(variableFound && equalsFound))
                    throw new Exception($"No X or equal sign found");

                while(stack.Count != 0){
                    PopToken = Pop();
                    queue.Enqueue(PopToken);
                }

                /*
                Console.Write("Postfix: ");

                foreach(Token token in queue){
                    Console.Write(token.Value);
                }
                    
                Console.WriteLine("\n");
                */

            } catch (Exception e){
                Console.WriteLine("ERROR: " + e.Message);
            }

            EvaulatePostfix();
        }

        private Token Peek(){
            return (Token)stack.Peek();
        }

        private Token Pop(){
            return (Token)stack.Pop();
        }

        // This function takes two operators, it will return true if top of the stack has higher or equal precedence to the current token
        private bool HasHigherPrec(int operatorOnStack, int token){
            return (operatorOnStack >= token);
        }


        public void EvaulatePostfix(){

            equalsFound = false;
            try{
                //int i = 0;
                foreach(Token token in queue){

                    // If token is a number, push it to the stack
                    if(token.isNumber(n) || token.isVariable()){
                        stack.Push(token);
                        // Else if token is an operator, pop one token from the stack, this will be the right operand
                        // pop another token from the stack, this will be the left operand
                    } else if (token.isAnOperator() || (token.isEquals() && !lookingForX)){

                        //Console.Write("Attempt: " + i++ + "\t");

                        if(token.isEquals()){
                            equalsFound = true;
                        } else {
                            operation = token.Value;
                            rightSide = Pop();
                            leftSide = Pop();
                        }

                        if(rightSide.isVariable()){
                            lookingForX = false;
                            right = 0;
                            left = Convert.ToInt32(leftSide.Value);
                            int otherresult = 0;

                            if(equalsFound){
                                Token PopResult = Pop();
                                otherresult = Convert.ToInt32(PopResult.Value);
                            } else {
                                continue;
                            }

                            if(!operation.Equals("/"))
                                n = 0;
                            else 
                                n = 1;

                            while(n < 10){
                                right = n;
                                result = Calculate(left, right, operation);
                                //Console.WriteLine("Attempt " + n + ": " + left + operation + right + "=" + result);
                                if(result == otherresult){
                                    XSolved = true;
                                    break;
                                }
                                n++;
                            } 

                            if(XSolved){
                                tokenResult = new Token(right.ToString());
                                stack.Push(tokenResult);
                            } else {
                                throw new Exception("Couldn't solve X.");
                            }
                            break;
                        }

                        if(leftSide.isVariable()){
                            lookingForX = false;
                            left = 0;
                            right = Convert.ToInt32(rightSide.Value);
                            int otherresult = 0;

                            if(equalsFound){
                                Token PopResult = Pop();
                                otherresult = Convert.ToInt32(PopResult.Value);
                            } else {
                                continue;
                            }

                            if(!operation.Equals("/"))
                                n = 0;
                            else 
                                n = 1;

                            while(n < 10){
                                left = n;
                                result = Calculate(right, left, operation);
                                //Console.WriteLine("Attempt " + n + ": " + left + operation + right + "=" + result);
                                if(result == otherresult){
                                    XSolved = true;
                                    break;
                                }
                                n++;
                            } 

                            if(XSolved){
                                tokenResult = new Token(left.ToString());
                                stack.Push(tokenResult);
                            } else {
                                throw new Exception("Couldn't solve X.");
                            }
                            break;
                        }

                        right = Convert.ToInt32(rightSide.Value);
                        left = Convert.ToInt32(leftSide.Value);
                        result = Calculate(left, right, operation);
                        //Console.WriteLine("Result of: " + left + operation + right + "=" + result);
                        tokenResult = new Token(result.ToString());
                        stack.Push(tokenResult);


                    } else if(token.isEquals()){
                        rightSide = Pop();
                        leftSide = Pop();

                        if(rightSide.isVariable()){
                            stack.Push(leftSide);
                            break;
                        }
                        if(leftSide.isVariable()) {
                            stack.Push(rightSide);
                            break;
                        }

                    }

                }

            } catch (Exception e){
                Console.WriteLine("Error: " + e.Message);
            }

        }

        public void PrintX(){
            if(stack.Count == 1){
                tokenResult = Pop();
                Console.WriteLine("X = " + tokenResult.Value);
            }
        }


        // Check the type of operator and perform calculations
        private int Calculate(int left, int right, string operation){
            switch(operation){
                case "+":
                    return (left + right);

                case "-":
                    return (left - right);

                case "*":
                    return (left * right);

                case "/":
                    return (left / right);

                case "%":
                    return (left % right);

                default:
                    throw new InvalidOperationException($"Unknown operator when calculating operation: {left} {operation} {right}");
            }
        }

        public void Display() {
            Console.WriteLine($"X = {result}");
        }
    }
}
