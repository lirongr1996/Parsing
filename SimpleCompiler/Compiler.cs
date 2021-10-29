using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCompiler
{
    public class Compiler
    {

        private Dictionary<string, int> m_dSymbolTable;
        private int m_cLocals;

        public Compiler()
        {
            m_dSymbolTable = new Dictionary<string, int>();
            m_cLocals = 0;

        }

        public List<string> Compile(string sInputFile)
        {
            List<string> lCodeLines = ReadFile(sInputFile);
            List<Token> lTokens = Tokenize(lCodeLines);
            TokensStack sTokens = new TokensStack();
            for (int i = lTokens.Count - 1; i >= 0; i--)
                sTokens.Push(lTokens[i]);
            JackProgram program = Parse(sTokens);
            return null;
        }

        private JackProgram Parse(TokensStack sTokens)
        {
            JackProgram program = new JackProgram();
            program.Parse(sTokens);
            return program;
        }

        public List<string> Compile(List<string> lLines)
        {

            List<string> lCompiledCode = new List<string>();
            foreach (string sExpression in lLines)
            {
                List<string> lAssembly = Compile(sExpression);
                lCompiledCode.Add("// " + sExpression);
                lCompiledCode.AddRange(lAssembly);
            }
            return lCompiledCode;
        }



        public List<string> ReadFile(string sFileName)
        {
            StreamReader sr = new StreamReader(sFileName);
            List<string> lCodeLines = new List<string>();
            while (!sr.EndOfStream)
            {
                lCodeLines.Add(sr.ReadLine());
            }
            sr.Close();
            return lCodeLines;
        }

        //Splits a string into a list of tokens, separated by delimiters
        private List<string> Split(string s, char[] aDelimiters)
        {
            List<string> lTokens = new List<string>();
            while (s.Length > 0)
            {
                string sToken = "";
                int i = 0;
                for (i = 0; i < s.Length; i++)
                {
                    if (aDelimiters.Contains(s[i]))
                    {
                        if (sToken.Length > 0)
                            lTokens.Add(sToken);
                        lTokens.Add(s[i] + "");
                        break;
                    }
                    else
                        sToken += s[i];
                }
                if (i == s.Length)
                {
                    lTokens.Add(sToken);
                    s = "";
                }
                else
                    s = s.Substring(i + 1);
            }
            return lTokens;
        }

        public List<Token> Tokenize(List<string> lCodeLines)
        {
            List<Token> lTokens = new List<Token>();
            //your code here
            Token t=new Token();
            int pos=0,x;
            List<string> splitLine=new List<string>();
            
            char [] aDelimiters=new char[21];
            char[] Separators = new char[] {  ',', ';'};
            char[] Parentheses = new char[] { '(', ')', '[', ']', '{', '}' };
            char[] Operators = new char[] { '*', '+', '-', '/', '<', '>', '&', '=', '|', '!' };
            Array.Copy(Operators,aDelimiters,10);
            Array.Copy(Parentheses,0,aDelimiters,10,6);
            Array.Copy(Separators,0,aDelimiters,16,2);
            aDelimiters[18]=' ';
            aDelimiters[19]='\t';
            aDelimiters[20]='.';

            for (int i=0;i<lCodeLines.Count;i++)
            {
                pos=0;
                splitLine=Split(lCodeLines[i],aDelimiters);
                if (splitLine[0]=="/"&&splitLine[1]=="/")
                    continue;
                for(int j=0;j<splitLine.Count;j++)
                {
                    if(splitLine[j]=="/"&&splitLine[j+1]=="/")
                        break;
                    if(t.getStatements().Contains(splitLine[j]))
                    {
                        lTokens.Add(new Statement(splitLine[j],i,pos));
                        pos+=splitLine[j].Length;
                    }
                    else if(t.getVarTypes().Contains(splitLine[j]))
                    {
                        lTokens.Add(new VarType(splitLine[j],i,pos));
                        pos+=splitLine[j].Length;
                    }
                    else if(t.getConstants().Contains(splitLine[j]))
                    {
                        lTokens.Add(new Constant(splitLine[j],i,pos));
                        pos+=splitLine[j].Length;
                    }
                    else if(t.getOperators().Contains(splitLine[j][0]))
                    {
                        lTokens.Add(new Operator(splitLine[j][0],i,pos));
                        pos+=splitLine[j].Length;
                    }
                    else if(t.getParentheses().Contains(splitLine[j][0]))
                    {
                        lTokens.Add(new Parentheses(splitLine[j][0],i,pos));
                        pos+=splitLine[j].Length;
                    }
                    else if(t.getSeparators().Contains(splitLine[j][0]))
                    {
                        lTokens.Add(new Separator(splitLine[j][0],i,pos));
                        pos+=splitLine[j].Length;
                    }
                    else if(int.TryParse(splitLine[j],out x))
                    {
                        lTokens.Add(new Number(splitLine[j],i,pos));
                        pos+=splitLine[j].Length;
                    }
                    else if((splitLine[j][0]>=65&&splitLine[j][0]<=90)||(splitLine[j][0]>=97&&splitLine[j][0]<=122))
                    {
                        lTokens.Add(new Identifier(splitLine[j],i,pos));
                        pos+=splitLine[j].Length;
                    }
                    else if(splitLine[j][0]>=48&&splitLine[j][0]<=57&&!int.TryParse(splitLine[j],out x))
                    {
                        throw new SyntaxErrorException("The identifier is not legal",new Identifier(splitLine[j],j,pos));
                    }
                    else if(splitLine[j][0]==' '||splitLine[j][0]=='\t')
                    {
                        pos+=splitLine[j].Length;
                        continue;
                    }
                    else if((splitLine[j][0] < 65 || splitLine[j][0] > 90) && (splitLine[j][0] < 97 || splitLine[j][0] > 122) && !int.TryParse(splitLine[j], out i))
                    {
                        throw new SyntaxErrorException("The operator is not legal",new Operator(splitLine[j][0],j,pos));
                    }
                    
                }
            }
            
            return lTokens;
        }

    }
}
