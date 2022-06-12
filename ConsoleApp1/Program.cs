using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Python.Runtime;

//https://stackoverflow.com/questions/69339974/python-net-pythonengine-initialize-crashes-application-without-throwing-except
//Note that running "pip install pythonnet" only installs the ability to load & use CLR types & assemblies from Python.
//    To embed PythonNet in a C# app, you actually don't need to install pythonnet on the Python side.


using System.IO;


namespace PythonExecutor
{
    class Program
    {    // 환경설정 Path를 설정하는 함수이다. 실제 Path가 바뀌는 건 아니고 프로그램 세션 안에서만 path를 변경해서 사용한다.
        public static void AddEnvPath(params string[] paths)
        {      // PC에 설정되어 있는 환경 변수를 가져온다.
            var envPaths = Environment.GetEnvironmentVariable("PATH").Split(Path.PathSeparator).ToList();
            // 중복 환경 변수가 없으면 list에 넣는다.
            envPaths.InsertRange(  0,   paths.Where( x => x.Length > 0 && !envPaths.Contains(x) ).ToArray()  );
            // 환경 변수를 다시 설정한다.
            Environment.SetEnvironmentVariable("PATH", string.Join(Path.PathSeparator.ToString(), envPaths), EnvironmentVariableTarget.Process);
        }

        // 시작 함수입니다.
        //

    //    namespace Python.Runtime
    //{
    //    public class Runtime
    //    {
    //        public Runtime();

    //        public static string? PythonDLL { get; set; }
    //        public static int MainManagedThreadId { get; }
    //        public static PyObject None { get; }

    //        public static int Py_Main(int argc, string[] argv);
    //        public static bool TryCollectingGarbage(int runs);
    //    }
    //}


    static void Main(string[] args)


        {
            Runtime.PythonDLL = @"C:\Users\moon\anaconda3\python38.dll";

            // 아까 where.exe python으로 나온 anaconda 설치 경로를 설정

            var PYTHON_HOME = Environment.ExpandEnvironmentVariables(@"C:\Users\moon\anaconda3\");
            // 환경 변수 설정
            AddEnvPath(PYTHON_HOME, Path.Combine(PYTHON_HOME, @"Library\bin")); 
            // Python 홈 설정.
            PythonEngine.PythonHome = PYTHON_HOME;
            // 모듈 패키지 패스 설정.
            PythonEngine.PythonPath = string.Join(

                Path.PathSeparator.ToString(),
                new string[] {
                  PythonEngine.PythonPath,
                    // pip하면 설치되는 패키지 폴더.
                     Path.Combine(PYTHON_HOME, @"Lib\site-packages"),  
                    // 개인 패키지 폴더
                     //"d:\\Python\\MyLib"
                     @"C:\Users\moon\source\repos\ConsoleApp1\ConsoleApp1\Python\MyLib"
                }
            );
            // Python 엔진 초기화
            PythonEngine.Initialize();     
            // Global Interpreter Lock을 취득
            using (Py.GIL())
            {
                // String 식으로 python 식을 작성, 실행
                PythonEngine.RunSimpleString(@"
                                    import sys; 
                                    print('hello world');
                                    print(sys.version); 
                  ");
                // 개인 패키지 폴더의 examples/calculator.py  를 읽어드린다.      examples is a package that contains __init__.pu file
                //    public static PyObject Import(string name);
                // dynamic np = Py.Import("numpy");
                dynamic test = Py.Import("examples.calculator");   // It uses  PythonEngine.PythonPath 
                // example/test.py의 Calculator 클래스를 선언
                dynamic f = test.Calculator(1, 2);        
                // Calculator의 add함수를 호출
                Console.WriteLine(f.add());
            }    // using GIL( Py.GIL() )
            // python 환경을 종료한다.
            PythonEngine.Shutdown();
            Console.WriteLine("Press any key...");
            Console.ReadKey();

        }   //   static void Main(string[] args)

    }   //class Program
} // namespace PythonExecutor







//namespace ConsoleApp1
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {
//        }


//public static void InitPython(Microsoft.Extensions.Logging.ILogger logger)
//{
//    string py_home = Program.PythonHome;
//    string py_path = $"{py_home};";

//    // will be different on linux/mac
//    string[] py_paths = {"DLLs", "lib", "lib/site-packages", "lib/site-packages/win32"
//        , "lib/site-packages/win32/lib", "lib/site-packages/Pythonwin" };

//    foreach (string p in py_paths)
//    {
//        py_path += $"{py_home}/{p};";
//    }

//    try
//    {
//        Runtime.PythonDLL = Program.PythonDll;
//        PythonEngine.PythonPath = $"{Program.ScriptsDir};{py_path}";
//        PythonEngine.PythonHome = Program.PythonHome;
//        PythonEngine.ProgramName = Program.ApplicationName;
//        PythonEngine.Initialize();
//        PythonEngine.BeginAllowThreads();

//        logger.LogInformation("Python Version: {v}, {dll}", PythonEngine.Version.Trim(), Runtime.PythonDLL);
//        logger.LogInformation("Python Home: {home}", PythonEngine.PythonHome);
//        logger.LogInformation("Python Path: {path}", PythonEngine.PythonPath);

//    }
//    catch (System.TypeInitializationException e)
//    {
//        throw new Exception($"FATAL, Unable to load Python, dll={Runtime.PythonDLL}", e);
//    }
//    catch (Exception e)
//    {
//        throw new Exception($"Python initialization Exception, {e.Message}", e);
//    }

//} // InitPython
// }   // class Program


//} // namespace
