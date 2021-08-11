using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.App.Utility.Java
{
    /// <summary>
    /// A wrapped .Net dll for weka.jar
    /// </summary>
    public class Weka
    {        
        public static string ClassifyTest(bool verbose = true)
        {
            const int percentSplit = 66;
            try
            {
                weka.core.Instances insts = new weka.core.Instances(new java.io.FileReader(@"Java/iris.arff"));
                insts.setClassIndex(insts.numAttributes() - 1);

                weka.classifiers.Classifier cl = new weka.classifiers.trees.J48();
                if (verbose) System.Console.WriteLine("Performing " + percentSplit + "% split evaluation.");

                //randomize the order of the instances in the dataset.
                weka.filters.Filter myRandom = new weka.filters.unsupervised.instance.Randomize();
                myRandom.setInputFormat(insts);
                insts = weka.filters.Filter.useFilter(insts, myRandom);

                int trainSize = insts.numInstances() * percentSplit / 100;
                int testSize = insts.numInstances() - trainSize;
                weka.core.Instances train = new weka.core.Instances(insts, 0, trainSize);

                cl.buildClassifier(train);
                int numCorrect = 0;
                for (int i = trainSize; i < insts.numInstances(); i++)
                {
                    weka.core.Instance currentInst = insts.instance(i);
                    double predictedClass = cl.classifyInstance(currentInst);
                    if (predictedClass == insts.instance(i).classValue())
                        numCorrect++;
                }
                var s = numCorrect + " out of " + testSize + " correct (" +
                           (double)((double)numCorrect / (double)testSize * 100.0) + "%)";
                if (verbose) System.Console.WriteLine(s);
                return s;
            }
            catch (java.lang.Exception ex)
            {
                ex.printStackTrace();
                return ex.toString();
            }
        }
    }
}
