
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Python.Runtime;
//using Numpy;
//using Numpy.Models;

//https://stackoverflow.com/questions/69339974/python-net-pythonengine-initialize-crashes-application-without-throwing-except
//Note that running "pip install pythonnet" only installs the ability to load & use CLR types & assemblies from Python.
//    To embed PythonNet in a C# app, you actually don't need to install pythonnet on the Python side.


using System.IO;


namespace PythonExecutor
{
    class Program
    {    // 환경설정 Path를 설정하는 함수이다. 실제 Path가 바뀌는 건 아니고 프로그램 세션 안에서만 path를 변경해서 사용한다.

        //public static void DisplayNDarray(NDarray nDarray, string name = "")
        //{
        //    Console.WriteLine($">> DisplayDetails {name}:");
        //    Console.WriteLine($"size {nDarray.size} shape {nDarray.shape} dims {nDarray.ndim}");
        //    Console.WriteLine($"mean {nDarray.mean()} std {nDarray.std()} min {nDarray.min()} max {nDarray.max()}");
        //}

        //public static void DisplayNDarrayData<TElementType>(NDarray nDarray)
        //{
        //    Console.WriteLine($"[{String.Join(", ", nDarray.GetData<TElementType>())}]");
        //}


        public static void AddEnvPath(params string[] paths)
        {      // PC에 설정되어 있는 환경 변수를 가져온다.
            var envPaths = Environment.GetEnvironmentVariable("PATH").Split(Path.PathSeparator).ToList();
            // 중복 환경 변수가 없으면 list에 넣는다.
            envPaths.InsertRange(0, paths.Where(x => x.Length > 0 && !envPaths.Contains(x)).ToArray());
            // 환경 변수를 다시 설정한다.
            Environment.SetEnvironmentVariable("PATH", string.Join(Path.PathSeparator.ToString(), envPaths), EnvironmentVariableTarget.Process);
        }


        public static void Main()

        {

            //Console.WriteLine("1 PythonPath={0}\n", PythonEngine.PythonPath);

            //Runtime.PythonDLL = @"C:\Users\moon\anaconda3\python38.dll";
            // Runtime.PythonDLL = @"C:\Users\moon\AppData\Local\Programs\Python\Python38\python38.dll";
            Environment.SetEnvironmentVariable("PYTHONNET_PYDLL", @"C:\Users\moon\AppData\Local\Programs\Python\Python38\python38.dll", EnvironmentVariableTarget.Process);


            //Console.WriteLine("1 PythonPath={0}\n", PythonEngine.PythonPath);

            //// Set the environment variables for Python: see https://github.com/pythonnet/pythonnet/issues/1348
            //string pathToVirtualEnv = "C:\\Users\\moon\\anaconda3\\envs\\gest_env";

            //Console.WriteLine("1 PythonPath={0}\n", PythonEngine.PythonPath);


            //Environment.SetEnvironmentVariable("PATH", pathToVirtualEnv, EnvironmentVariableTarget.Process);

            //Console.WriteLine("2 PythonPath={0}\n", PythonEngine.PythonPath);

            //Environment.SetEnvironmentVariable("PYTHONHOME", pathToVirtualEnv, EnvironmentVariableTarget.Process);

            //Console.WriteLine("3 PythonPath={0}\n", PythonEngine.PythonPath);

            //Environment.SetEnvironmentVariable("PYTHONPATH", $"{pathToVirtualEnv}\\Lib\\site-packages;{pathToVirtualEnv}\\Lib;" +
            //       "D:\\Dropbox\\metaverse\\PythonnetTest\\ConsoleApp1\\Python\\MyLib;"   +
            //       "D:\\Dropbox\\metaverse\\gesticulator;" + 
            //        "D:\\Dropbox\\metaverse\\gesticulator\\gesticulator;" + 
            //         "D:\\Dropbox\\metaverse\\gesticulator\\gesticulator\\visualization",
            //                                     EnvironmentVariableTarget.Process);




            //// 아까 where.exe python으로 나온 anaconda 설치 경로를 설정

            //var PYTHON_HOME = Environment.ExpandEnvironmentVariables(@"C:\Users\moon\anaconda3");

            var PYTHON_HOME = Environment.ExpandEnvironmentVariables(@"C:\Users\moon\AppData\Local\Programs\Python\Python38");

            // http://www.delphibasics.co.uk/Method.asp?NameSpace=System&Class=Environment&Type=Class&Method=ExpandEnvironmentVariables

            Console.WriteLine(" PythonHome={0}\n", PYTHON_HOME);
            //// 환경 변수 설정
            //AddEnvPath(PYTHON_HOME, Path.Combine(PYTHON_HOME, @"Library\bin"));  // Add path to PATH
            // Python 홈 설정.

            //Console.WriteLine("4 PythonPath={0}\n", PythonEngine.PythonPath);
            //PythonEngine.PythonHome = pathToVirtualEnv;


            //Console.WriteLine("5 PythonPath={0}\n", PythonEngine.PythonPath);

            //ythonEngine.PythonPath = Environment.GetEnvironmentVariable("PYTHONPATH", EnvironmentVariableTarget.Process);

            //Console.WriteLine("6 PythonPath={0}\n", PythonEngine.PythonPath);

            // 모듈 패키지 패스 설정.
            PythonEngine.PythonPath = string.Join(

                Path.PathSeparator.ToString(),
                new string[] {
                  PythonEngine.PythonPath,
                    // pip하면 설치되는 패키지 폴더.
                     Path.Combine(PYTHON_HOME, @"Lib\site-packages"),  
                    // 개인 패키지 폴더
                     @"D:\Dropbox\metaverse\PythonnetTest\ConsoleApp1\Python\MyLib",   // calculator.py module
                     @"D:\Dropbox\metaverse\gesticulator",  // the project root folder  under which demo package resides; demo package has demo.py module
                     @"D:\Dropbox\metaverse\gesticulator\gesticulator\visualization"   // the project root folder for motion visualization

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
                //public static PyObject Import(string name);

                //Actually, you just need to set the Api Compatibility Level to .Net 4.x in your Player Settings, if you need to use the dynamic keyword.

                dynamic pysys = Py.Import("sys");   // import sys module from  PythonEngine.PythonPath 
                dynamic pySysPath = pysys.path;
                string[] sysPathArray = (string[])pySysPath;    // About conversion: https://csharp.hotexamples.com/site/file?hash=0x7a3b7b993fab126a5a205be68df1c82bd87e4de081aa0f5ad36909b54f95e3d7&fullName=&project=pythonnet/pythonnet

                List<string> sysPath = ((string[])pySysPath).ToList<string>();
                Console.WriteLine(pysys.path);
                Console.WriteLine(pySysPath);
                Console.WriteLine(sysPath);
                //Since the List collection class implements the IEnumerable interface, we are allowed to use the foreach loop to iterate its content.

                sysPath.ForEach(i => Console.Write("{0}\t", i));   // https://stackoverflow.com/questions/52927/console-writeline-and-generic-list

                for (int i = 0; i<sysPathArray.Length; i++)
                {
                    Console.Write("{0}\t", sysPath[i]);
                }


                dynamic os = Py.Import("os");    // All python objects should be declared as dynamic type: https://discoverdot.net/projects/pythonnet

               

                dynamic pycwd = os.getcwd();
                string cwd = (string)pycwd;

                Console.WriteLine("\n\n initial os.cwd={0}", cwd);



                os.chdir(@"D:\Dropbox\metaverse\gesticulator\demo");
                pycwd = os.getcwd();
                cwd = (string)pycwd;

                Console.WriteLine("\n\n new os.cwd={0}", cwd, "\n\n");



                dynamic np = Py.Import("numpy");



                Console.WriteLine("\n\n np.array test#\n");
                //// create an NDarray from a C# array : https://github.com/SciSharp/Numpy.NET
                var a = np.array(new[] { 2, 4, 9, 25 });
                Console.WriteLine("a:{0} ", a);
                // apply the square root to each element
                var roots = np.sqrt(a);
                Console.WriteLine("roots:{0}", roots);
                // array([1.41421356, 2.        , 3.        , 5.        ])


                Console.WriteLine("\n\n np.array test#\n");
                //// create an NDarray from a C# array : https://github.com/SciSharp/Numpy.NET
                var b = np.array(new List<float> { 2, 4, 9, 25 });
                Console.WriteLine("b:{0} ", a);
                // apply the square root to each element
                roots = np.sqrt(b);
                Console.WriteLine("roots:{0}", roots);
                // array([1.41421356, 2.        , 3.        , 5.        ])

                dynamic mod = Py.Import("examples.calculator");


                dynamic pyresult = mod.add(1, 2);

                int result = (int)pyresult;  //  Microsoft.CSharp.RuntimeBinder.RuntimeBinderException:: Cannot convert type 'Python.Runtime.PyObject' to 'int'

                Console.WriteLine("\n pythion result:{0}", pyresult);




                ///////////////////////////
                //  (1)  The original version where the input audio file and text are  read from within python code
                dynamic demo = Py.Import("demo.demo");   


                dynamic motionPythonArray = demo.main();       // returns an 2 dim numpy array

                
                // https://community.intersystems.com/post/embedded-python-and-tcl-tkinter-windows:

                // If your embedded python code calls tkinter library (which is used by a lot of graphic producing libraries, including matplotlib), you might get this error:


                // https://mail.python.org/pipermail/pythondotnet/2013-July/001390.html


                //=>  C# dynamic object support / Easy calling from C#
                // Conversion: https://github.com/pythonnet/pythonnet/wiki/Codecs:-customizing-object-marshalling-between-.NET-and-Python

                // A lot of pythonnet code examples: https://github.com/pythonnet/pythonnet/tree/925c1660d73883b9636c27d3732c328a321cebb8/src/embed_tests



                //C# PyObject is holding a Python object handle (which are pointers into C heap). Internally Python objects count references to themselves.

                //In C# you can either force releasing the handle by calling PyObject.Dispose, in which case Python object refcount is decreased immediately, 
                //                    and if at this point it reaches 0, Python object will be destroyed and deallocated (from C heap).



                // float[,,] motionFromPythonArray = motionFromPython.AsManagedObject(typeof(float[,,,]));
                // float[,,] motionFromPythonArray = (float[,,])motionFromPython.As<float[,,,]>();      //     motionFromPython is numpy.ndarray => cannot be converted to System.Single
                //Spam s = (Spam)ob.AsManagedObject(typeof(Spam));;

                // https://github.com/SciSharp/Numpy.NET

                //There are some closed issues on converting from python objects to CLR objects dynamically on the pythonnet github here:
                //github.com / pythonnet / pythonnet / issues / 484 and here github.com / pythonnet / pythonnet / issues / 623
                //Basically this guy wrote his own converter and stated an example on the issue,
                //but you can find his code on github.com / yagweb / pythonnetLab / blob / master / pynetLab /… as well as
                //the example in the TestPyConverter file github.com / yagweb / pythonnetLab / blob / master / Test /… – 
                //eyoT


                //https://github.com/yagweb/pythonnetLab/blob/master/Test/TestPyConverter.cs



                //   self.output_dim = 45

                // https://stackoverflow.com/questions/66866731/numpy-net-getting-values



                // float[,] motionArray = new float[520, 45];

                //  Converts a Python object to a Python list if possible, raising a PythonException if the conversion is not possible.This is equivalent to the Python expression "list(object)".
                PyList motionPyList = PyList.AsList(motionPythonArray);   // https://stackoverflow.com/questions/63621696/python-net-convert-python-list-to-net-list-c



                //float[,] motionArray = (float[,])motionPythonList;     // does not work

                Console.WriteLine("\n\n Print Python List  in Console#\n");
                //PyList motionPyList = (PyList) motionPythonList;     // This does not work

                for (int i = 0; i < 520; i++)
                {
                    Console.WriteLine("{0}: \n", i);
                    for (int j = 0; j < 45; j++)
                    {
                        //motionArray[i,j] = (float)motionPythonList[i][j];
                        Console.Write($"{motionPyList[i][j]} \t");



                    }

                    Console.WriteLine("\n");

                }



                // (2) Test: the new version where the input audio and text are prepared within the csharp code

                string text = "Deep learning is an algorithm inspired by how the human brain works, and as a result it's an algorithm which has no theoretical limitations on what it can do. The more data you give it and the more computation time you give it, the better it gets. The New York Times also showed in this article another extraordinary result of deep learning which I'm going to show you now. It shows that computers can listen and understand.";

              //# audio, sample_rate = librosa.load(audio_filename) # sample_rate = 22050 discrete values  per second; 0.1 s = 1 frame => 2205 discrete time points per frame
                dynamic librosa = Py.Import("librosa");  // import a package

                dynamic audio_sample_rate = librosa.load(@"D:\Dropbox\metaverse\gesticulator\demo\input\jeremy_howard.wav"); // audio is an np.array [ shape=(n,) or (2,n)]
                                                                                                                             //   array([ -4.756e-06,  -6.020e-06, ...,  -1.040e-06,   0.000e+00], dtype=float32)


                //     cf:  np.ndarray((2,), buffer = np.array([1, 2, 3]),
                //           offset = np.int_().itemsize,
                //           dtype = int) # offset = 1*itemsize, i.e. skip first element
                //          => array([2, 3])

                //numpy.array(object, dtype = None, *, copy = True, order = 'K', subok = False, ndmin = 0, like = None), where object should be array-like


                // int sample_rate_cs = (int)sample_rate;
                //Beginning with C# 3, variables that are declared at method scope can have an implicit "type" var.
                //    An implicitly typed local variable is strongly typed just as if you had declared the type yourself,
                //    but the compiler determines the type. The following two declarations of i are functionally equivalent:


                //var i = 10; // Implicitly typed.
                //int i = 10; // Explicitly typed.

                //var xs = new List<int>(); <==>   List<int> xs = new List<int>();

                //https://www.geeksforgeeks.org/dynamic-type-in-c-sharp/#:~:text=In%20C%23%204.0%2C%20a%20new,type%20at%20the%20run%20time.
                //=> 
                //In C# 4.0, a new type is introduced that is known as a dynamic type. It is used to avoid the compile-time type checking.
                //    The compiler does not check the type of the dynamic type variable at compile time,
                //    instead of this, the compiler gets the type at the run time. The dynamic type variable is created using dynamic keyword.

                var audio = audio_sample_rate[0];

                //https://stackoverflow.com/questions/23143184/how-do-i-check-type-of-dynamic-datatype-at-runtime

                Type t = (audio).GetType();

                Console.WriteLine("\n\n (audio).GetType():{0}\n", t);

                //PyList  audio2 = PyList.AsList( audio) ;  //or dynamic audio = audio_sample_rate[0]
                //PyList audio2 = audio.As<PyList>();  //or dynamic audio = audio_sample_rate[0]:audio is not a list
                int sample_rate = audio_sample_rate[1];
               
                dynamic democs = Py.Import("demo.democs");

                motionPythonArray = democs.main(audio, text, sample_rate);
                Type t2 = (motionPythonArray).GetType();

                Console.WriteLine("\n\n ( motionPythonArray).GetType():{0}\n", t2);




                //float[,] motionPythonNpArray = np.array(motionPythonArray);

                //var  motionPythonNpArray = np.array(motionPythonArray);

                motionPyList = PyList.AsList(motionPythonArray);
                // float[,] motionArrayCs = (float[,])motionPyList;


                //Console.WriteLine("\n\n Print Python List  in Console:  Passing input to gesticulator from csharp\n");


                for (int i = 0; i < 520; i++)
                {
                    Console.WriteLine("{0}: \n", i);
                    for (int j = 0; j < 45; j++)
                    {
                        //motionArray[i,j] = (float)motionPythonList[i][j];
                        //Console.Write($"{motionPyList[i][j]} \t");
                        Console.Write($"{motionPythonArray[i][j]} \t");
                       



                    }

                    Console.WriteLine("\n");

                }

                //Generate a sequence of gestures based on a sequence of speech features (audio and text)

                //Args:
                //    audio [N, T, D_a]:    a batch of sequences of audio features
                //    text  [N, T, D_t]:  a batch of sequences of text BERT embedding
                //    use_conditioning:     a flag indicating if we are using autoregressive conditioning
                //    motion: [N, T, D_m]   the true motion corresponding to the input (NOTE: it can be None during testing and validation)
                //    use_teacher_forcing:  a flag indicating if we use teacher forcing

                //Returns:
                //    motion [N, T, D_m]:   a batch of corresponding motion sequences




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
                // Debug.LogFormat("\n HI. args.model_file:{0}\n\n", arg_model_file);

                // Calculator의 add함수를 호출
                //Console.WriteLine(f.add());
            }    // using GIL( Py.GIL() )


            // python 환경을 종료한다.
            PythonEngine.Shutdown();
            //Console.WriteLine("Press any key...");
            //Console.ReadKey();
        } // void static Main()


    }  //class Program


}//  namespace PythonExecutor





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


