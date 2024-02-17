using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPAlgo
{

    public class Node 
    {
        private int count;
        private bool visited;
        
        public Dictionary<string, Node> item = new Dictionary<string, Node>();

        public Node()
        {
            count = 0;
            visited = false;
        }
        public int Count { get; set; }
        public bool Visited { get; set; }


        
    }
    public class Tree
    {
        public Node root;
        private int minSupportCount;

        public Tree()
        {
            root = new Node();
            root.Count = 0;
            root.item = null;

            minSupportCount = 13024;
        }



        public void BuildTree(ref List<string> elemsRecvd)
        {
            if (root.item == null)
            {
                Node crawler = root;
                foreach (string element in elemsRecvd)
                {
                    Node node = new Node();

                    node.Count = 1;

                    crawler.item = new Dictionary<string, Node>();
                    crawler.item[element] = node;
                    crawler = node;
                }
           
            }
            else 
            {
                Node crawler = root;
                if (crawler.item != null)
                {
                    foreach (string element in elemsRecvd)
                    {
                        if (crawler.item.ContainsKey(element))
                        {
                            Node node = crawler.item[element];
                            node.Count = node.Count + 1;
                            crawler.item[element] = node;
                            crawler = node;
                        }
                        else
                        {
                            Node node = new Node();
                            node.Count = 1;
                            crawler.item.Add(element, node);
                            crawler = node;
                        }
                    }
                }

                
            }
           
           
        }

       
        public List<string> generateFrequentItemset(Node rootNode, List<string> frequentPattern)
        {
            
            


            foreach (KeyValuePair<string, Node> item in rootNode.item)
            {

                if (!item.Value.Visited && item.Value.Count > minSupportCount)
                {
                    item.Value.Visited = true;
                    frequentPattern.Add(item.Key);
                    generateFrequentItemset(item.Value, frequentPattern);
                }
            }


            return frequentPattern;
        }

    }

    class FPGrowth
    {
        private const int minSupportCount = 6512;


        private static List<List<Object>> returnC1(ref List<String> Database)
        {
            List<List<Object>> itemSets = new List<List<Object>>();
            foreach (String transaction in Database)
            {
                String[] transItems = transaction.Split(',');


                int countOfTable = 0;
                foreach (String itemName in transItems)
                {

                    string itemNameTemp = itemName.Trim();
                    Boolean itemInTable = false;
                    for (int itemCount = 0; itemCount < itemSets.Count; itemCount++)
                    {
                        if (itemSets[itemCount][0].ToString() == itemNameTemp)
                        {
                            int countOfItemName = Convert.ToInt32(itemSets[itemCount][1]);
                            itemSets[itemCount][1] = ++countOfItemName;
                            itemInTable = true;
                        }

                    }

                    if (itemInTable == false)
                    {
                        List<Object> newItemSet = new List<Object>();
                        newItemSet.Add(itemNameTemp);
                        newItemSet.Add(1);

                        itemSets.Add(newItemSet);
                    }

                    countOfTable++;
                }
            }

            return itemSets;
        }
        private static List<List<Object>> returnL1(ref List<List<Object>> Ck)
        {
            List<List<Object>> L1Items = new List<List<Object>>();
            for (int count = 0; count < Ck.Count; count++)
            {
                if (Convert.ToInt32(Ck[count][1]) > minSupportCount)
                {
                    L1Items.Add(Ck[count]);
                }
            }
            return L1Items;
        }
        private static void sortL1Descending(ref List<List<Object>> L1)
        {
            List<List<Object>> decendingSortedL1 = new List<List<Object>>();

            for (int i = 0; i < L1.Count; i++)
            {
                for (int j = 0; j < L1.Count -1; j++)
                {

                    if (Convert.ToInt32(L1[j][1]) < Convert.ToInt32(L1[j + 1][1]))
                    {
                        List<Object> temp = new List<Object>();
                        temp = L1[j];
                        L1[j] = L1[j + 1];
                        L1[j + 1] = temp;
                    }
                }
            }               

        }
        private static List<String> PreProcessing(string[] Database)
        {
            List<String> preProcessedDB = new List<String>();
            int lineNum = 0;
            foreach (String trans in Database)
            {
                String[] items = trans.Split(' ');
                int index = 0;
                String newTrans = "";
                foreach (String item in items)
                {
                    String itemTemp = item.Trim();
                    if (itemTemp != "")
                    {
                        if (index > 10)
                            continue;
                            newTrans = newTrans + ", " + item;
                        index++;
                    }
                }
                lineNum++;

                preProcessedDB.Add(newTrans);
            }

            return preProcessedDB;

        }

        static void Main(string[] args)
        {
            string[] Database = System.IO.File.ReadAllLines(@"..\..\..\Dataset.txt");
            //Preprocessing on Data
         

            List<String> processedDatabase = new List<String>();
            processedDatabase = PreProcessing(Database);

            List<List<Object>> C1ItemList = new List<List<Object>>();


            List<List<Object>> L1ItemList = new List<List<Object>>();

            System.Diagnostics.Stopwatch calcTime = System.Diagnostics.Stopwatch.StartNew();
            C1ItemList = returnC1(ref processedDatabase);
            L1ItemList = returnL1(ref C1ItemList);

            sortL1Descending(ref L1ItemList);


            //Create Tree class obj
            Tree tree = new Tree();

            //for transaction that are frequent
            List<string> elementsFoundList = new List<string>();

            //read database and create the tree based
            foreach (String trans in processedDatabase)
            {
                for(int i = 0; i< L1ItemList.Count; i++)
                {     
                    //if transaction contains frequent itemset add it
                    if (trans.Contains(L1ItemList[i][0].ToString()))
                    {
                        elementsFoundList.Add(L1ItemList[i][0].ToString());
                    }
                }
                tree.BuildTree(ref elementsFoundList);
                elementsFoundList.Clear();

            }


            List<string> freqItems = new List<string>();

            //generate frequent itemset
            freqItems = tree.generateFrequentItemset(tree.root, freqItems);


            Console.WriteLine("The Most Frequent Item Set in Data is: \n");
            /*Print frequent Itemset*/
            foreach (string item in freqItems)
            {
                Console.WriteLine(" "+item);
            }

            /*Stop calculating time after algorithm finishes*/
            Console.WriteLine("\nTotal Execution Time is  " + ((calcTime.ElapsedMilliseconds)/1000) + " seconds \n");

            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }

        }
    }
}
