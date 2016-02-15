
/*
 * Zaawansowane metody optymalizacyjne 
 * Beata Jaroszewska 128476 
 * & Małgorzta Łyczywek
 * 
 * Luty 2016
 * 
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
        }
        bool greedy = false; // dokladny
        bool heuristic = false; 
        int number_of_tasks ; // liczba zadan do uszeregowania
        int current_task_mach_1 = 0; // aktualna zrobiona liczba zadan
      
        int time_machine_1 = 0; // czas uszeregowania na maszynie 1 
        int time_machine_2 = 0; // czas uszeregowania na maszynie 2 
        // machine 1 
        int distance_from_last_break_machine_1 = 0; // moment od ostatniej przerwy na maszynie 
        int max_distance_mach_1 = 0; //  maxymalna odleglosc miedzy przerwami na maszynie 1 
        int time_of_break_mach1 = 0; // czas przerwy na maszynie 1 
        int TIME_after_all_op_1 = 0;
        // machine  2 
        int distance_from_last_break_machine_2 = 0;
        int max_distance_mach_2 = 0;
        int time_of_break_mach_2 = 0;
        int sum_rest_op_2 = 0;
        int  last_task_on_2 = 0;
        // heurystyka 
        List<task> best = new List<task>(); // lista do tej pory najlepiej uszeregowanych zadan!
        List<task> during_iteration = new List<task>();  // do tego co bedziemy sobie sprawdzac
        List<task> SWAP = new List<task>();  // pomocnicza do SWAPA 
        List<ruch> TABU = new List<ruch>();  // LISTA TABU  
        int number_iteration = 0; 

        private void pictureBox2_Click(object sender, EventArgs e)
        {

            if (greedy == true && heuristic == false)
            {
                label4.Text = "algorytm dokładny";

            }
            else if (heuristic == true && greedy == false)
            {
                label4.Text = "algorytm heurystyczny";
            }
            else if (heuristic && greedy)
            {
                label4.Text = "dokładny i heurystyka";
            }
           
            label5.Text = System.Convert.ToString(number_of_tasks);
          
            List<task> parts = new List<task>(); // lista tych naszych zadan! 
            //<---! kreacja operacji dla podanej liczby zadan! --->
            
             Random r = new Random();
             for (int i = 1; i <= number_of_tasks; i++)
             {
                 int time_op_1 = r.Next(1, 10);
                 int time_op_2=(r.Next(1,10));
                 parts.Add(new task() { op_1 = time_op_1, op_2 = time_op_2, number = i });
             }
            
             
            /*
            //<!--- Zadania  WPROWADZANE NA SZTYWNO -->
            parts.Add(new task() { op_1 = 2, op_2 = 8, number = 1 });
            parts.Add(new task() { op_1 = 5, op_2 = 6, number = 2 });
            parts.Add(new task() { op_1 = 7, op_2 = 1, number = 3 });
            parts.Add(new task() { op_1 = 4, op_2 = 9, number = 4 });
            parts.Add(new task() { op_1 = 1, op_2 = 7, number = 5 });
             */
            //number_of_tasks = 5; 
           // parts.Add(new task() { op_1 = 2, op_2 = 7, number = 6 });
            //parts.Add(new task() { op_1 = 3, op_2 = 1, number = 7 });
            //parts.Add(new task() { op_1 = 9, op_2 = 3, number = 8 });
            //parts.Add(new task() { op_1 = 3, op_2 = 1, number = 9 });
            //parts.Add(new task() { op_1 = 5, op_2 = 2, number = 10 });
            if (greedy && !heuristic)
            {
                var watch = Stopwatch.StartNew();
                // the code that you want to measure comes here
               
                foreach (task aPart in parts)
                {
                    listBox2.Items.Add("Zadanie: " + aPart.number + " Czas operacji 1: " + aPart.op_1 + " Czas operacji 2: " + aPart.op_2);
                    sum_rest_op_2 += aPart.op_2; // sumujemy wszystkie opercje 2 ! 
                }

                List<task> short_op1 = new List<task>(); // lista zadac op_1<op_2
                List<task> long_op1 = new List<task>(); // lista zadac op_1<op_2
                // JOHNSONS START wkladanie zadan ze wzgledu na czas operacji 
                foreach (task aPart in parts)
                {
                    //Form set1 containing all the jobs with p1j < p2j
                    if (aPart.op_1 <= aPart.op_2)
                    {
                        short_op1.Add(aPart);
                        //  label11.Text = String.Join("  ", aPart.number);
                    }
                    else //Form set2 containing all the jobs with p1j > p2j,the jobs with p1j=p2j may be put in either set.
                    {
                        long_op1.Add(aPart);
                    }

                }

                //The job in set1 go first in the sequence and they go in increasing order of op_1(SPT)
                List<task> SortedListShort = short_op1.OrderBy(o => o.op_1).ToList(); //SET1
                List<task> SortedListLong = long_op1.OrderByDescending(o => o.op_2).ToList(); //SET2
                List<task> All_task = SortedListShort.Concat(SortedListLong).ToList();
                machine_1<task>(All_task);
                var last_task = All_task[All_task.Count - 1];
                last_task_on_2 = last_task.op_2;
                TIME_after_all_op_1 = time_machine_1; // bo to aktualny po tym wykonaniu 
                machine_2<task>(All_task);
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;

                foreach (task aPart in All_task)
                {
                    listBox1.Items.Add("Zadanie: " + aPart.number);

                }
                label12.Text = System.Convert.ToString(time_machine_2); // wyswietlanie czasu uszeregowan 
                label13.Text = System.Convert.ToString(time_machine_1);
                label8.Text = System.Convert.ToString(elapsedMs);

            }

            if (heuristic && !greedy)
            { 
           foreach (task aPart in parts)
                {
                    listBox2.Items.Add("Zadanie: " + aPart.number + " Czas operacji 1: " + aPart.op_1 + " Czas operacji 2: " + aPart.op_2);
                   
                }

                foreach (task aPart in parts) 
                {  
                    best.Add(aPart);
                   
                   
                }
                var watch = Stopwatch.StartNew();
                heuristic_algo<task>(best);
                machine_1<task>(best);
                var last_task =best[best.Count - 1];
                last_task_on_2 = last_task.op_2;
               
               
                TIME_after_all_op_1 = time_machine_1; // bo to aktualny po tym wykonaniu 
                machine_2<task>(best);
                watch.Stop(); // koniec mierzenia czasu 
                var elapsedMs = watch.ElapsedMilliseconds;
                label25.Text = System.Convert.ToString(time_machine_2); // wyswietlanie czasu uszeregowan 
                label24.Text = System.Convert.ToString(time_machine_1);
                foreach (task aPart in best)
                {
                    listBox3.Items.Add("Zadanie: " + aPart.number);
                    
                }
                label27.Text = System.Convert.ToString(elapsedMs);
            }
            if (heuristic && greedy)
            {

                var watch_g = Stopwatch.StartNew();
                // the code that you want to measure comes here

                foreach (task aPart in parts)
                {
                    listBox2.Items.Add("Zadanie: " + aPart.number + " Czas operacji 1: " + aPart.op_1 + " Czas operacji 2: " + aPart.op_2);
                    sum_rest_op_2 += aPart.op_2; // sumujemy wszystkie opercje 2 ! 
                }

                List<task> short_op1 = new List<task>(); // lista zadac op_1<op_2
                List<task> long_op1 = new List<task>(); // lista zadac op_1<op_2
                // JOHNSONS START wkladanie zadan ze wzgledu na czas operacji 
                foreach (task aPart in parts)
                {
                    //Form set1 containing all the jobs with p1j < p2j
                    if (aPart.op_1 <= aPart.op_2)
                    {
                        short_op1.Add(aPart);
                        //  label11.Text = String.Join("  ", aPart.number);
                    }
                    else //Form set2 containing all the jobs with p1j > p2j,the jobs with p1j=p2j may be put in either set.
                    {
                        long_op1.Add(aPart);
                    }

                }

                //The job in set1 go first in the sequence and they go in increasing order of op_1(SPT)
                List<task> SortedListShort = short_op1.OrderBy(o => o.op_1).ToList(); //SET1
                List<task> SortedListLong = long_op1.OrderByDescending(o => o.op_2).ToList(); //SET2
                List<task> All_task = SortedListShort.Concat(SortedListLong).ToList();
                machine_1<task>(All_task);
                var last_task = All_task[All_task.Count - 1];
                last_task_on_2 = last_task.op_2;
                TIME_after_all_op_1 = time_machine_1; // bo to aktualny po tym wykonaniu 
                machine_2<task>(All_task);
                watch_g.Stop();
                var elapsedMs_g = watch_g.ElapsedMilliseconds;

                foreach (task aPart in All_task)
                {
                    listBox1.Items.Add("Zadanie: " + aPart.number);

                }
                label12.Text = System.Convert.ToString(time_machine_2); // wyswietlanie czasu uszeregowan 
                label13.Text = System.Convert.ToString(time_machine_1);
                label8.Text = System.Convert.ToString(elapsedMs_g);


                //heuristic: 
                time_machine_1 = 0;
                time_machine_2 = 0;
                

                foreach (task aPart in parts)
                {
                    best.Add(aPart);


                }
                var watch_h = Stopwatch.StartNew();
                heuristic_algo<task>(best);
                machine_1<task>(best);
                var last_task_h = best[best.Count - 1];
                last_task_on_2 = last_task_h.op_2;


                TIME_after_all_op_1 = time_machine_1; // bo to aktualny po tym wykonaniu 
                machine_2<task>(best);
                watch_h.Stop(); // koniec mierzenia czasu 
                var elapsedMs_h = watch_h.ElapsedMilliseconds;
                label25.Text = System.Convert.ToString(time_machine_2); // wyswietlanie czasu uszeregowan 
                label24.Text = System.Convert.ToString(time_machine_1);
                foreach (task aPart in best)
                {
                    listBox3.Items.Add("Zadanie: " + aPart.number);

                }
                label27.Text = System.Convert.ToString(elapsedMs_h);
            }
            label5.Text = System.Convert.ToString(number_of_tasks);
          
            
        }

        private void label1_Click(object sender, EventArgs e) // wybrane algorytmy 
        {

        }
     
        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e) // zadania do uszeregowania 
        {
            number_of_tasks = (int)numericUpDown1.Value; // pobrana ilosc zadan do uszeregowania 

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        

       

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            max_distance_mach_1 = (int)numericUpDown2.Value; 
        }
        
        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            time_of_break_mach1 = (int)numericUpDown4.Value;
        }
        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            max_distance_mach_2 = (int)(numericUpDown3.Value);
        }

        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            time_of_break_mach_2=(int)(numericUpDown5.Value);
        }


        void machine_1<T>(List<task> data)
        {
            distance_from_last_break_machine_1 = 0;
           foreach(task aPart  in data ) {
               // MACHINE 1: 
               current_task_mach_1 += 1;
             

               if (aPart.op_1 + distance_from_last_break_machine_1 <= max_distance_mach_1)
               {
                   time_machine_1 += aPart.op_1;
                   distance_from_last_break_machine_1 += aPart.op_1;
                
               }
               else
               {
                   //  sprawdzamy ile mozemy wykonac przed przerwa i to dodajemy a reszte dodajemy po przerwie 
                  
                   int before_break = max_distance_mach_1 - distance_from_last_break_machine_1;

                   int after_break = aPart.op_1 - before_break;
                   time_machine_1 += before_break;
                   before_break = 0;
                  
                   if ((after_break != 0 && number_of_tasks == current_task_mach_1) || number_of_tasks != current_task_mach_1)
                   {
                       time_machine_1 += time_of_break_mach1;
                     
                       distance_from_last_break_machine_1 = 0; 
                   }

                   distance_from_last_break_machine_1 = 0;   // zawsze po dodaniu przerwy kasujemy dystans!!!
                
                   while (after_break > max_distance_mach_1) // jezeli wlasnie wpada w kilka przerw bo przekracza ten maxymalny dystans! 
                   {
                       before_break = max_distance_mach_1 - distance_from_last_break_machine_1; //tyle moze sie wykonac przed przerwa kolejna 
                       after_break = after_break - before_break;
                       time_machine_1 += before_break;
                       time_machine_1 += time_of_break_mach1;
                       distance_from_last_break_machine_1 = 0;
                       before_break = 0;
                     
                   }
                   //teraz zeby dodawalo ten czas ktory pozostal z ostaniej przerwy w danym zadaniu!
                   time_machine_1 += after_break;
                
                   distance_from_last_break_machine_1 = after_break;
                
           
               }
               
           } // for each aPart 
        }

        void machine_2<T>(List<task> data)
        {
            int current_task_mach_1 = 0; // aktualna zrobiona liczba zadan
         
            int time_machine_1 = 0; // czas uszeregowania na maszynie 1 
    
            // machine 1 
            int distance_from_last_break_machine_1 = 0; // moment od ostatniej przerwy na maszynie 
            

            // machine  2 
            int distance_from_last_break_machine_2 = 0;
   
            foreach (task aPart in data)
            {
                // MACHINE 1: 
                current_task_mach_1 += 1;
               
                if (aPart.op_1 + distance_from_last_break_machine_1 <= max_distance_mach_1)
                {
                    time_machine_1 += aPart.op_1;
                    distance_from_last_break_machine_1 += aPart.op_1;
                }
                else
                {
                    //  sprawdzamy ile mozemy wykonac przed przerwa i to dodajemy a reszte dodajemy po przerwie 
                
                    int before_break = max_distance_mach_1 - distance_from_last_break_machine_1;

                    int after_break = aPart.op_1 - before_break;
                    time_machine_1 += before_break;
                    before_break = 0;
                    if ((after_break != 0 && number_of_tasks == current_task_mach_1) || number_of_tasks != current_task_mach_1)
                    {
                        time_machine_1 += time_of_break_mach1;
                        distance_from_last_break_machine_1 = 0;
                    }

                    distance_from_last_break_machine_1 = 0;   // zawsze po dodaniu przerwy kasujemy dystans!!!
                  
                    while (after_break > max_distance_mach_1) // jezeli wlasnie wpada w kilka przerw bo przekracza ten maxymalny dystans! 
                    {
                        before_break = max_distance_mach_1 - distance_from_last_break_machine_1; //tyle moze sie wykonac przed przerwa kolejna 
                        after_break = after_break - before_break;
                        time_machine_1 += before_break;
                        time_machine_1 += time_of_break_mach1;
                        distance_from_last_break_machine_1 = 0;
                        before_break = 0;
                       
                    }
                    //teraz zeby dodawalo ten czas ktory pozostal z ostaniej przerwy w danym zadaniu!
                    time_machine_1 += after_break;
                  
                    distance_from_last_break_machine_1 = after_break;
             
                }

                ///MACHINE 2: 
                int dif = 0;
       
                // iczy suma reszty operacji nie jest krotsza od maksymalnego mozliwego wstawienie przerwy 
                if (time_machine_2 < time_machine_1 && (distance_from_last_break_machine_2 +last_task_on_2  + (TIME_after_all_op_1- time_machine_2 )> max_distance_mach_2))// zeby nie wstawialo 2 przerw obok siebie 
                {
                    if ((time_machine_1 - time_machine_2)-time_of_break_mach_2 + distance_from_last_break_machine_2 > max_distance_mach_2)
                    {
                        

                        if ((time_machine_1 - time_machine_2) % time_of_break_mach_2 > distance_from_last_break_machine_2)
                        {
                         
                            distance_from_last_break_machine_2 = time_of_break_mach_2 - ((time_machine_1 - time_machine_2) % time_of_break_mach_2);
                        }
                        else
                        {
                            distance_from_last_break_machine_2 = 0;
                        }
                        
                    }
                    else {
                           distance_from_last_break_machine_2 = 0; 
                    }

                    time_machine_2 += time_of_break_mach_2; 
               
                    int difff = 0;
                    if (time_machine_2 < time_machine_1)
                    {
                        difff = time_machine_1 - time_machine_2;
                        time_machine_2 += difff;

                    }

                
                }
                    
                else if ((time_machine_2 < time_machine_1)) 
                {
                    dif = time_machine_1 - time_machine_2;
                    time_machine_2 += dif;
                    distance_from_last_break_machine_2 +=dif; 
               
                }
                if (aPart.op_2 + distance_from_last_break_machine_2 <= max_distance_mach_2)
                {
                    time_machine_2 += aPart.op_2;
                    distance_from_last_break_machine_2 += aPart.op_2;
                    sum_rest_op_2 -= aPart.op_2;
                
                }

                else
                {
                    int before_break = max_distance_mach_2 - distance_from_last_break_machine_2;

                    int after_break = aPart.op_2 - before_break;

                    time_machine_2 += before_break;
                    before_break = 0;
                    if ((after_break != 0 && number_of_tasks == current_task_mach_1) || number_of_tasks != current_task_mach_1)
                    {
                        time_machine_2 += time_of_break_mach_2;
                        distance_from_last_break_machine_2 = 0;
                    }

                    distance_from_last_break_machine_2 = 0;   // zawsze po dodaniu przerwy kasujemy dystans!!!

                    while (after_break > max_distance_mach_2) // jezeli wlasnie wpada w kilka przerw bo przekracza ten maxymalny dystans! 
                    {
                        before_break = max_distance_mach_2 - distance_from_last_break_machine_2; //tyle moze sie wykonac przed przerwa kolejna 
                        after_break = after_break - before_break;
                        time_machine_2 += before_break;
                        time_machine_2 += time_of_break_mach_2;
                        distance_from_last_break_machine_2 = 0;
                        before_break = 0;

                    }
                    //teraz zeby dodawalo ten czas ktory pozostal z ostaniej przerwy w danym zadaniu!
                    time_machine_2 += after_break;

                    distance_from_last_break_machine_2 = after_break;
                    sum_rest_op_2 -= aPart.op_2; // odejmujemy cala opercje nie dzielimy na przed przerwa i po przerwie bo nie czekamy na zadne zadanie juz bo wykonujemy swoje 
                

                }

               
            } // for each aPart 
        }

        void heuristic_algo<T>(List<task> data)
        {   
            // liczymy czas uszeregowania dla aktualnego rozwiazania: 
            int time_machine_1_h = 0;
            int time_machine_2_h = 0; // nasza wartosc uszeregowania w pierwsej iteracji byle jakiego! 
            foreach (task aPart in data) // dodanie kazdej operacji do listy  
            {
                during_iteration.Add(aPart); //dodaje to co jest w bescie  
            }
      
            foreach (task aPart in data) // licze czas dla besta 
            {
                time_machine_1_h += aPart.op_1;
                if (time_machine_2_h >= time_machine_1_h)
                {
                    time_machine_2_h += aPart.op_2;

                }
                else
                {
                    int gap = 0;
                    gap = time_machine_1_h - time_machine_2_h;
                    time_machine_2_h += gap;
                    time_machine_2_h += aPart.op_2;
                }
            }
      
            int size_of_list = 0;
            size_of_list = best.Count();


            int time_for_tabu = (size_of_list / 5) + 3; // kiedy usuwamy zadanie z TABU 
            // czas na tabu musi byc logiczny z liczba zadan czyli dla +5 nie wstawiajmy mneisjzej ilosci zadan niz 5 
         
            int current_tabu_time = 0;
    
            while (number_iteration < 200) 
            { 
               
                Random r = new Random();
                int start_h= r.Next(1, size_of_list);
                int end_h = r.Next(1, size_of_list);
                
                while (end_h == start_h ) // jezeli losujemy te sama to wylosuj inna do zmianY! lub losujemy z tabu! 
                {
                    end_h = r.Next(1, size_of_list);
                }
               
                int counter =0;
                
                while (counter<TABU.Count() ) // ze chociaz jedno musi byc inne nie bedace w tabu takiego ukladu 
                {
                   
                  //  Console.WriteLine("wszedlem w while" + number_iteration);
                  // 1) czy koniec =/= poczatek - jesli rowna losuj ponownie
                    // 2)  na liscie tabu jest taka para - jesli jest losuj ponownie
                    
                    foreach (ruch m in TABU) // czy nie  znajduje sie w tabu ten ruch 
                    {   
                        if((m.end!=end_h || m.start!=start_h) && end_h!=start_h) {
                            counter += 1;
                            
                        }
                       
                        else
                        {
                            counter = 0;
                            end_h = r.Next(1, size_of_list);
                            start_h = r.Next(1, size_of_list);

                        }
                       
                        continue;
                       
                    }
           
                } //while tabu counter 
               
                TABU.Add(new ruch() { start = start_h, end=end_h }); // dodaje do tabu to SOBIE
               // zamieniaj miejscami: 
                SWAP.Add(during_iteration[start_h]); // dodaje ten pierwszy element do zmiany 
                SWAP.Add(during_iteration[end_h]); 
                during_iteration[end_h] = SWAP[0]; // dodaje to co ze zrobilam
                during_iteration[start_h] = SWAP[1];

                SWAP.Clear();
                int time_during_1 = 0; // czas dla aktualnego rozwiazania 
                int time_during_2 = 0;
                foreach (task aPart in during_iteration) // licze czas dla aktualnego rozwiazania 
                {
                   time_during_1 += aPart.op_1;
                    if (time_during_2 >= time_during_1)
                    {
                        time_during_2 += aPart.op_2;

                    }
                    else
                    {
                        int gap = 0;
                        gap =time_during_1- time_during_2;
                       time_during_2 += gap;
                       time_during_2 += aPart.op_2;
                    }
                }
             ;
                if( time_during_2<time_machine_2_h) // jezeli aktualnie znalezione jest lepsze 
                {
                    data.Clear();

                    foreach (task aPart in during_iteration) // dodanie kazdej operacji do listy  
                    {
                        data.Add(aPart); //dodaje to co jest w bescie  
                    }
                    time_machine_2_h = time_during_2; // aktualizuje czasy na krotsze 
                    time_machine_1_h = time_during_1; // aktualizuje czasy
              
                }

              
                if (current_tabu_time >= time_for_tabu) //usuwamy pierwszy z tabu 
                {
                    TABU.RemoveAt(0);
                }
                number_iteration += 1;
                current_tabu_time += 1;
                // czy usuwam cos z Tabu? 
            }//while number iteration 



        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (greedy == false)
            {
                greedy = true;
            }
            else
            { greedy = false; }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (heuristic == false)
            {
                heuristic = true;
            }

            else
            {
                heuristic = false;
            }

        }

        private void label28_Click(object sender, EventArgs e)
        {

        }

    }
    public class task // musi byc jako druga klasa bo visual wariuje! 
    {
        public int op_1 { get; set; } // dlugosc operacji pierwszej 
        public int op_2 { get; set; } // dlugosc operacji drugiej 
        public int number { get; set; } // numer zadania 
    }
    public class ruch
    {
        public int start =0;
        public int end =0; 
    }
}
