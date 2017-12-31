﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RogerWaters.RealTimeDb.Linq2Sql;

namespace RogerWaters.RealTimeDb.TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            DoWithDb();
            //DoWithDbStressTest();
            //DefaultContex();
            Console.WriteLine("Enter to close");
            Console.ReadLine();
        }

        public static void DoWithDb()
        {
            using (
            var context = new RealtimeDbDataContextBuilder<SomeDbDataContext>(() => new SomeDbDataContext()).Build()
            )
            {
                Console.WriteLine(DateTime.Now.TimeOfDay);
                var queries =
                    Enumerable.Range(0, 1000).AsParallel().Select(i => context.Query
                    (
                        c =>
                            from m in c.MyTable
                            join m2 in c.MyTable2 on m.some equals m2.some
                            where m.some != null && m2.some != "asd"
                            where m.some == "Merge"
                            select new
                            {
                                m.MyTable_id,
                                m.some,
                                m2.MyTable2_id,
                                Some2 = m2.some
                            },
                        r => new
                        {
                            r.MyTable_id,
                            r.MyTable2_id
                        }
                    )).ToArray();
                Console.WriteLine("Queries applied");
                Console.ReadLine();
                queries.AsParallel().ForAll(q => q.Result.Dispose());
                Console.WriteLine("Queries disposed");
            }
        }

        public static void DoWithDbStressTest()
        {
            using (var context = new RealtimeDbDataContextBuilder<TPCCDataContext>(() => new TPCCDataContext()).Build())
            {
                Console.ReadLine();
                context.DisposeBehavior = DisposeBehavior.CleanupSchema;
            }
        }
    }
}
