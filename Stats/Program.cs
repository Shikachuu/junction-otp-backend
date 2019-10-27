using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stats
{

    class Proba
    {
        static Random random = new Random();

        public double avgWaitTime = 0;
        public Proba(int personCount)
        {

            //random.NextDouble();

            List<double> arrivalTimes = new List<double>();
            for (int i = 0; i < personCount; ++i)
            {
                arrivalTimes.Add(random.NextDouble());
            }
            arrivalTimes.Sort();

            List<double> departureTimes = new List<double>();
            for (int i = 0; i < personCount; ++i)
            {
                departureTimes.Add(0);
            }

            departureTimes[0] = arrivalTimes[0];
            for (int i = 1; i < personCount; ++i)
            {
                departureTimes[i] = Math.Max(departureTimes[i - 1], arrivalTimes[i]) + 1;
            }

            List<double> waitTimes = new List<double>();
            for (int i = 0; i < personCount; ++i)
            {
                waitTimes.Add(0);
            }

            for (int i = 0; i < personCount; ++i)
            {
                waitTimes[i] = departureTimes[i] - arrivalTimes[i];
            }

            double sum = 0;
            for (int i = 0; i < personCount; ++i)
            {
                sum += waitTimes[i];
            }


            avgWaitTime = sum / personCount;
        }

    }
    class Program
    {
        static void Main(string[] args)
        {


            List<double> l = new List<double>(70);

            for(int i=1; i < 70; ++i )
            {
                double probaSum = 0;
                for(int j = 0; j < 100; ++j)
                {
                    Proba proba = new Proba(i);
                    probaSum += proba.avgWaitTime;
                }
                probaSum /= 100;

                //l[i] = probaSum;
                Console.WriteLine($"tabla[{i}] = {probaSum};");
            }
            
        }


    }
}
