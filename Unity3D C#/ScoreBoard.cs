using UnityEngine;
using System.Collections;
using System;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour {

	private string connectionString;
	public int lvlScore;
	public int lvlSc;
	public int[] lvlc=new int[3];
	public Text lvl1txt;
	public Text lvl2txt;
	public Text lvl3txt;

	public Canvas cv;
	// Use this for initialization
	void Start () {
		connectionString = "URI=file:" + Application.dataPath + "frustrated.sqlite";
		cv.enabled = false;
	}
	
	// Update is called once per frame

	public void setScore(int lvlNow, int newScore){
		connectionString = "URI=file:" + Application.dataPath + "frustrated.sqlite";
		using (IDbConnection dbConnection = new SqliteConnection (connectionString)) {
			dbConnection.Open ();
			print ("Connected");
			using (IDbCommand dbCmd = dbConnection.CreateCommand ()) {
				string sqlQuery = String.Format("UPDATE 'HighScore' SET Score={1} WHERE Level={0} AND Score<{1};", lvlNow, newScore);
				dbCmd.CommandText = sqlQuery;
				dbCmd.ExecuteScalar ();
				dbConnection.Close ();
			}
		}
	}
	public void showScore(){
		cv.enabled=true;
		for(int i=0;i<3;i++){
			lvlc[i]=GetScores(i+1);
		}
		lvl1txt.text="L e v e l  1 : "+lvlc[0].ToString();
		lvl2txt.text="L e v e l  2 : "+lvlc[1].ToString();
		lvl3txt.text="L e v e l  3 : "+lvlc[2].ToString();
	}

	public void closeScore(){
		cv.enabled=false;
	}

	public int GetScores(int lvl){
		using (IDbConnection dbConnection = new SqliteConnection (connectionString)) {
			dbConnection.Open ();
			using (IDbCommand dbCmd = dbConnection.CreateCommand ()) {
				string sqlQuery = String.Format("SELECT * FROM HighScore WHERE Level={0}", lvl);
				dbCmd.CommandText = sqlQuery;
				using (IDataReader reader = dbCmd.ExecuteReader ()) {
					while (reader.Read ()) {
						lvlSc=reader.GetInt32 (1);
						print (lvlSc);
					}
					dbConnection.Close ();
					reader.Close ();
				}
			}
		}
		return lvlSc;
	}
}
