using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DWG
{
    class Database
    {
        private bool disposed;
        public NpgsqlConnection connection;
        public Database()
        {
            string ip = "127.0.0.1";
            string port = "5432";
            string user = "zew";
            string password = "zewng";
            string database = "zew2020";

            connection = new NpgsqlConnection($"Server={ip}; Port={port}; User Id={user}; Password={password}; Database={database}");
            
                connection.Open();
        }

        ~Database()
        {
            this.Dispose(false);
            
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources here.
                    connection.Close();
                }

                // Dispose unmanaged resources here.
            }

            disposed = true;
        }

    public int InsertZNE(int sys, int grp, string name, string btn)
        {
            NpgsqlParameter sys_id = new NpgsqlParameter("sys_id", DbType.Int32);
            NpgsqlParameter sys_num = new NpgsqlParameter("sys_num", DbType.Int32);
            NpgsqlParameter grp_id = new NpgsqlParameter("grp_id", DbType.Int32);
            NpgsqlParameter dsc = new NpgsqlParameter("dsc", DbType.String);
            NpgsqlParameter btn_dsc = new NpgsqlParameter("btn_dsc", DbType.String);

            sys_id.Value = sys;
            sys_num.Value = NextSys_num();
            grp_id.Value = grp;
            if(name == "")
            {
                name = "TEST";
            }
            if(btn == "")
            {
                btn = "TEST";
            }
            dsc.Value = name;
            btn_dsc.Value = btn;
            string insert = "INSERT INTO public.zne(sys_id,sys_num,grp_id,dsc,btn_dsc,lastchgts) VALUES(:sys_id,:sys_num,:grp_id,:dsc,:btn_dsc,current_timestamp) RETURNING id";
            NpgsqlCommand cmd = new NpgsqlCommand(insert, connection);

            cmd.Parameters.Add(sys_id);
            cmd.Parameters.Add(sys_num);
            cmd.Parameters.Add(grp_id);
            cmd.Parameters.Add(dsc);
            cmd.Parameters.Add(btn_dsc);

            int id = (int)cmd.ExecuteScalar();

            return id;
        }

        public int NextSys_num()
        {
            try
            {
                string select = "SELECT sys_num FROM public.zne ORDER BY sys_num DESC LIMIT 1";
                NpgsqlCommand cmd = new NpgsqlCommand(select, connection);
                int result = (int)cmd.ExecuteScalar();
                
                result++;
                return result;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }

        }

        public int IconID(string name, double rotation)
        {
            rotation = Math.Round(rotation);
            string direction = string.Empty;
            if(rotation >= -10 && rotation <= 20)
            {
                direction = "_e";
            } else if(rotation >= 80 && rotation <= 100)
            {
                direction = "_n";
            } else if(rotation >= 170 && rotation <= 190)
            {
                direction = "_w";
            } else if(rotation >= 260 && rotation <= 280)
            {
                direction = "_s";
            } else if(rotation >= 300 && rotation <= 320)
            {
                direction = "_se";
            } else if(rotation >= 130 && rotation <= 150)
            {
                direction = "_nw";
            } else if(rotation >= 30 && rotation <= 50)
            {
                direction = "_ne";
            } else if(rotation >= 210 && rotation <= 230)
            {
                direction = "_sw";
            }

            try {
                name = name + direction;
                string select = "SELECT id,fname FROM gis.ico";
                NpgsqlCommand cmd = new NpgsqlCommand(select, connection);
                cmd.Prepare();
                using (NpgsqlDataReader dr = cmd.ExecuteReader())
                {
                    string icon = string.Empty;
                    while (dr.Read())
                    {
                        icon = dr["fname"].ToString();
                        icon = Path.GetFileNameWithoutExtension(icon);
                        
                        if(name.IndexOf(icon)>=0)
                        {
                            return (int)dr["id"];
                        }
                    }

                    return 0;
                }
                
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        public void InsertMap_zne(int mapID, double positionX, double positionY, double rotation, string name, string btn_name)
        {
            NpgsqlParameter map_id = new NpgsqlParameter("map_id", DbType.Int32);
            NpgsqlParameter zne_id = new NpgsqlParameter("zne_id", DbType.Int32);
            NpgsqlParameter x = new NpgsqlParameter("x", DbType.Double);
            NpgsqlParameter y = new NpgsqlParameter("y", DbType.Double);
            NpgsqlParameter w = new NpgsqlParameter("w", DbType.Int32);
            NpgsqlParameter h = new NpgsqlParameter("h", DbType.Int32);
            NpgsqlParameter ico_id = new NpgsqlParameter("ico_id", DbType.Int32);

            ico_id.Value = IconID(name, rotation);
            map_id.Value = mapID;
            zne_id.Value = InsertZNE(191, 1280, name, btn_name);
            x.Value = positionX - 12;
            y.Value = 1080 - positionY - 12;
            w.Value = 25;
            h.Value = 25;
            
            string insert = "INSERT INTO gis.map_zne(map_id,zne_id,x,y,w,h,ico_id) VALUES(:map_id,:zne_id,:x,:y,:w,:h,:ico_id)";
            NpgsqlCommand cmd = new NpgsqlCommand(insert, connection);
            cmd.Parameters.Add(map_id);
            cmd.Parameters.Add(zne_id);
            cmd.Parameters.Add(x);
            cmd.Parameters.Add(y);
            cmd.Parameters.Add(w);
            cmd.Parameters.Add(h);
            cmd.Parameters.Add(ico_id);

            cmd.Prepare();
            cmd.ExecuteNonQuery();
            connection.Close();
        }
    }
}
