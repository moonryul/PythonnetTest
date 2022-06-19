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
            Runtime.PythonDLL = @"C:\Users\moon\AppData\Local\Programs\Python\Python38\Python38.dll";

            // 아까 where.exe python으로 나온 anaconda 설치 경로를 설정

            var PYTHON_HOME = Environment.ExpandEnvironmentVariables(@"C:\Users\moon\AppData\Local\Programs\Python\Python38");
            // 환경 변수 설정
            //AddEnvPath(PYTHON_HOME, Path.Combine(PYTHON_HOME, @"Library\bin"));  // Add path to PATH
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
                     @"C:\Users\moon\source\repos\ConsoleApp1\ConsoleApp1\Python\MyLib",
                      @"D:\Dropbox\metaverse\ConsoleApp1\ConsoleApp1\Python\gesticulator",  // the root folder itself  under which demo package resides; demo package has demo.py module
                      @"D:\Dropbox\metaverse\ConsoleApp1\ConsoleApp1\Python\gesticulator\gesticulator",
                       @"D:\Dropbox\metaverse\ConsoleApp1\ConsoleApp1\Python\gesticulator\gesticulator\visualization"
                    
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
print(sys.version); ");
                // 개인 패키지 폴더의 examples/calculator.py  를 읽어드린다.      examples is a package that contains __init__.pu file
                //    public static PyObject Import(string name);

                dynamic pysys = Py.Import("sys");   // It uses  PythonEngine.PythonPath 
                dynamic pySysPath = pysys.path;
                string[] sysPathArray = ( string[]) pySysPath;    // About conversion: https://csharp.hotexamples.com/site/file?hash=0x7a3b7b993fab126a5a205be68df1c82bd87e4de081aa0f5ad36909b54f95e3d7&fullName=&project=pythonnet/pythonnet

                List<string> sysPath = ((string[])pySysPath).ToList<string>();
                //Console.WriteLine(pysys.path);
                //Console.WriteLine(pySysPath);
                // Console.WriteLine(sysPath); 
                // Since the List collection class implements the IEnumerable interface, we are allowed to use the foreach loop to iterate its content.

                // sysPath.ForEach(i => Console.Write("{0}\t", i));   // https://stackoverflow.com/questions/52927/console-writeline-and-generic-list

                //for (int i = 0; i < sysPathArray.Length; i++)
                //{
                //    Console.Write("{0}\t", sysPath[i]);
                //}

                Console.WriteLine("\nsys.path:\n");
                Array.ForEach(sysPathArray, element =>  Console.Write("{0}\t", element));

                //Console.WriteLine(sysPathArray);

                // All python objects should be declared as dynamic type: https://discoverdot.net/projects/pythonnet

                dynamic os = Py.Import("os");

                dynamic pycwd = os.getcwd();
                string cwd = (string)pycwd;

                Console.WriteLine("\n\n initial os.cwd={0}", cwd);



                //os.chdir(@"D:\Dropbox\metaverse\ConsoleApp1\ConsoleApp1\Python\gesticulator\demo");
                //pycwd = os.getcwd();
                //cwd = (string)pycwd;

                //Console.WriteLine("\n\n new os.cwd={0}", cwd, "\n\n");


                //dynamic np = Py.Import("numpy");

                dynamic mod = Py.Import("examples.calculator");
                

                dynamic  pyresult =  mod.getStr();
                string result = (string) pyresult;

                //string   result = (string)pyresult;  //  Microsoft.CSharp.RuntimeBinder.RuntimeBinderException:: Cannot convert type 'Python.Runtime.PyObject' to 'int'

                Console.WriteLine("\n pythion result:{0}", pyresult);   

                Console.WriteLine("\n pythion result:{0}", result);



                dynamic demo = Py.Import("demo.demo");   // It uses  PythonEngine.PythonPath 
                Console.WriteLine("\n demo module:{0}\n", demo);

               
                dynamic arg_model_file  = demo.main();

                //string strresult = (string) arg_model_file;         // https://github.com/pythonnet/pythonnet/issues/451
                                                                    // python dynamically typed: Yes
                                                                    // C# dynamically typed: No, strongly typed. It uses lots of overloads to
                                                                    // return the required type. =>  Moreover, C# 4.0 is dynamically  typed too:
                                                                    // https://pythondotnet.python.narkive.com/4SDbJ9lz/python-net-dynamic-types-of-returns-pyobject-from-the-runtime
                                                                    // https://csharpdoc.hotexamples.com/class/Python.Runtime/PyObject


                // passing array: https://stackoverflow.com/questions/64990129/how-to-pass-array-to-a-function-in-net-using-pythonnet:

                //Try initializing it like this;

                //using (Py.GIL())
                //{
                //    trendln = Py.Import("trendln");
                //    dynamic h = new float[] { 1F, 2F, 3F };
                //    int a, b = trendln.calc_support_resistance(h);
                //}

                //https://github.com/pythonnet/pythonnet/issues/484

                //using (Py.GIL())
                //{
                //    var scope = Py.CreateScope();
                //    scope.Exec(
                //         "a=[1, \"2\"]"
                //    );
                //    dynamic a = scope.Get("a");
                //    object cc = a[0];
                //    Console.WriteLine(cc.GetType()); //print PyObject
                //    Console.WriteLine(cc.GetType() == typeof(PyInt)); //print false
                //    Console.WriteLine(cc);
                //    scope.Dispose();
                //}


                // Net to Python type conversions summary: https://github.com/pythonnet/pythonnet/issues/623
                //https://zditect.com/code/python/using-pythonnet-to-interface-csharp-library.html


                //Console.WriteLine("\n args.model_file:{0}", strresult);
                Console.WriteLine("\n HI. args.model_file:{0}\n\n", arg_model_file);

                // Calculator의 add함수를 호출
                //Console.WriteLine(f.add());
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
