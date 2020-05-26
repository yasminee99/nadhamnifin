﻿using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Image = System.Drawing.Image;

namespace ProjetNadhamni
{
    public partial class DailyPlanning : Form
    {
        public DailyPlanning()
        {
            InitializeComponent();
        }

        SqlConnection con = new SqlConnection("Data Source=DESKTOP-69MM1NJ\\SQLEXPRESS;Initial Catalog=NadhamniDB;Integrated Security=True;Pooling=False");
        SqlCommand cmd = new SqlCommand();
        


        private void DailyPlanning_Shown(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                string selectQuery = "select ProfilePic from Profile  where UserName = '" + Home.FK + "'";
                SqlCommand cmd = new SqlCommand(selectQuery, con);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                if (dr.HasRows)
                {
                    Byte[] img = (Byte[])(dr[0]);
                    if (img == null)
                    {
                        circularPic1.Image = null;
                    }

                    else
                    {
                        MemoryStream ms = new MemoryStream(img);
                        circularPic1.Image = Image.FromStream(ms);

                    }
                }
                else
                {   //interface
                    MessageBox.Show("You don't pocess an image ");
                }
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message); }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }

            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("select * from Tasks where  DateOfTask='" + (DateTime.Today).ToString("yyyy-MM-dd") + "' and UserName = '" + Home.FK + "'", con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    ViewTasks.Rows.Add(dr[0], dr[2], dr[1], Convert.ToDateTime(dr[3]).ToShortDateString(), dr[4], dr[5], dr[7], dr[8], dr[6], dr[9]);
                    
                        }
                dr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }

           //firstname shown
            try
            {

                con.Open();
                SqlCommand cmd = new SqlCommand("select FirstName from Profile where UserName='" + Home.FK + "'", con);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    txt_firstName.Text = dr[0].ToString();


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }

        }
        public void exportgridtopdf(DataGridView dgv, String filename)
        {
            BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1250, BaseFont.EMBEDDED);
            PdfPTable pdftable = new PdfPTable(dgv.Columns.Count);
            pdftable.DefaultCell.Padding = 3;
            pdftable.WidthPercentage = 100;
            pdftable.HorizontalAlignment = Element.ALIGN_LEFT;
            pdftable.DefaultCell.BorderWidth = 1;
            iTextSharp.text.Font text = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.NORMAL);
            //Add header
            foreach (DataGridViewColumn column in dgv.Columns)
            {
                PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText, text));
                cell.BackgroundColor = new iTextSharp.text.BaseColor(240, 240, 240);
                pdftable.AddCell(cell);
            }
            //ADD datroow
            foreach (DataGridViewRow row in dgv.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value == null)
                    { MessageBox.Show("loading"); }
                    else
                        pdftable.AddCell(new Phrase(cell.Value.ToString(), text));
                }
            }
            var savefiledilogue = new SaveFileDialog();
            savefiledilogue.FileName = filename;
            savefiledilogue.DefaultExt = ".pdf";
            if (savefiledilogue.ShowDialog() == DialogResult.OK)
            {
                using (FileStream stream = new FileStream(savefiledilogue.FileName, FileMode.Create))
                {
                    Document pdfdocument = new Document(PageSize.A4, 10, 10f, 10f, 0f);
                    PdfWriter.GetInstance(pdfdocument, stream);
                    pdfdocument.Open();
                    pdfdocument.Add(pdftable);
                    pdfdocument.Close();
                    stream.Close();

                }

            }
        }

      

        private void btn_SettingProfile_Click(object sender, EventArgs e)
        {
            Settings st2 = new Settings();
            this.Hide();
            st2.Show();
        }

        private void btn_statProfile_Click(object sender, EventArgs e)
        {
            this.Hide();
            Statistics stat = new Statistics();
            
            stat.Show();
        }

        private void btn_taskProfile_Click(object sender, EventArgs e)
        {
            Tasks tsk = new Tasks();
            this.Hide();
            tsk.Show();
        }

        private void btn_profilePro_Click(object sender, EventArgs e)
        {
            Profile pro = new Profile();
            this.Hide();
            pro.Show();
        }

        private void btn_dashProfile_Click(object sender, EventArgs e)
        {
            Dashboard dsh = new Dashboard();
            this.Hide();
            dsh.Show();
        }

        private void btn_print_Click(object sender, EventArgs e)
        {
            exportgridtopdf(ViewTasks, "planing");
        }

        private void DailyPlanning_Load(object sender, EventArgs e)
        {

        }

        private void ExitDashboard_Click(object sender, EventArgs e)
        {
            this.Hide();
            Welcome w = new Welcome();
            w.Show();
        }
    }
}
