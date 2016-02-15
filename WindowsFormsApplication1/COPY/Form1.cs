using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        bool greedy = false;
        bool heuristic = false;
        int number_of_tasks=0;
        int time_off_machine_1=0;
        int time_off_machine_2=0;
        int time_on_machine_1 = 0;
        
        private void button1_Click(object sender, EventArgs e) // START przycisk
        {
           
            time_unaviable_machine();
            if (greedy == true && heuristic==false)
            {
                label4.Text = "algorytm zachłanny";

            }
            else if (heuristic == true && greedy==false)
            {
                label4.Text = "algorytm heurystyczny";
            }
            else if (heuristic== true && greedy==true) {
                label4.Text = "algorytm zachłanny oraz heurystyczny";
            }

            label5.Text = System.Convert.ToString(number_of_tasks);

           // label8.Text =(text);
            
          //  MessageBox.Show("Super! Program działa!");
        }
        private void chart1_Click(object sender, EventArgs e) //wykres
        {

        }

    

        private void label1_Click(object sender, EventArgs e)
        {

        }


        private void checkBox1_CheckedChanged_1(object sender, EventArgs e) 
        {   if (greedy==false)
            {
                greedy = true; 
            }
            else
            {greedy = false; }
           
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

      
        private  void time_unaviable_machine()
        {
            Random r = new Random();
            time_off_machine_1=(r.Next(5, 10));

            time_off_machine_2=(r.Next(5, 10));
          //  label1.Text = System.Convert.ToString(time_off_machine_2);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            number_of_tasks = (int)numericUpDown1.Value; // pobrana ilosc zadan do uszeregowania 
           
        }

        
    }
}
