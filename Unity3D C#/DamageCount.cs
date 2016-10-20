using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.SceneManagement;
using System.Data;
using Mono.Data.Sqlite;

public class DamageCount : MonoBehaviour {
	public Text Text1;
	public Text Text2;
	public int score;
	public GameObject g1;
	public GameObject g2;
	public GameObject g3;
	public GameObject g4;
	public GameObject g5;

	public int trial;
	public float velo;
	public int mx;
	public int tmx;
	public int lvlNow;
	public Canvas cv;

	public Rigidbody2D projectile;			//	The rigidbody of the projectile
	public float resetSpeed = 0.05f;		//	The angular velocity threshold of the projectile, below which our game will reset

	private float resetSpeedSqr;			//	The square value of Reset Speed, for efficient calculation
	public GameObject springObj;			//	The SpringJoint2D component which is destroyed when the projectile is launched
	public GameObject projectile2;
	private SpringJoint2D spring;
	public Rigidbody2D connectTo;
	public bool trialbro;
	public GameObject destination;

	private Rigidbody2D tempProj;

	private bool gt1, gt2, gt3, gt4, gt5;
	private bool kia;
	// Use this for initialization
	void Start () {
		score = 0;
		ScoreCard (Text1);
		Text2.text = "";
		gt1 = true;
		gt2 = true;
		gt3 = true;
		gt4 = true;
		gt5 = true;
		cv.enabled = false;
		trial = 0;
		velo = projectile.velocity.sqrMagnitude;
		//	Calculate the Resset Speed Squared from the Reset Speed
		resetSpeedSqr = resetSpeed * resetSpeed;
		//	Get the SpringJoint2D component through our reference to the GameObject's Rigidbody
		tempProj = projectile;
		projectile2.SetActive (false);
	}
	void ScoreCard(Text tx){
		tx.text = "Score: " + (score*100).ToString ();
	}

	public void killed(){
		score += 1;
		kia = true;
	}
		
	public void PauseMenuOpen(){
		cv.enabled = true;
	}

	public void PauseMenuClose(){
		cv.enabled = false;
	}

	// Update is called once per frame
	void Update () {

		velo = tempProj.velocity.sqrMagnitude;
		trialbro = velo < resetSpeedSqr;
		if (!g1.GetComponent<SpriteRenderer>().enabled&&gt1) {
			killed ();
			gt1 = false;
		}

		if (!g2.GetComponent<SpriteRenderer>().enabled && gt2) {
			killed ();
			gt2 = false;
		}

		if (lvlNow == 2) {
			if (!g3.GetComponent<SpriteRenderer>().enabled&&gt3) {
				killed ();
				gt3 = false;
			}

			if (!g4.GetComponent<SpriteRenderer>().enabled && gt5) {
				killed ();
				gt4 = false;
			}
			if (!g5.GetComponent<SpriteRenderer>().enabled&&gt5) {
				killed ();
				gt5 = false;
			}
		}

		//	If the spring had been destroyed (indicating we have launched the projectile) and our projectile's velocity is below the threshold...
		if (tempProj.GetComponent<SpringJoint2D>()==null&&trialbro&&trial<tmx) {
			//	... call the Reset() function
			print("HERE");
			trial+=1;
			trialbro = false;
			checkScore ();
			projectile2.SetActive(true);
			tempProj = projectile2.GetComponent<Rigidbody2D> ();
		}

		if (kia) {
			ScoreCard (Text1);
			kia = false;
		}

		checkScore ();
	}

	void setScore(int newScore){
		string connectionString = "URI=file:" + Application.dataPath + "frustrated.sqlite";
		using (IDbConnection dbConnection = new SqliteConnection (connectionString)) {
			dbConnection.Open ();
			using (IDbCommand dbCmd = dbConnection.CreateCommand ()) {
				string sqlQuery = String.Format("UPDATE 'HighScore' SET Score={1} WHERE Level={0} AND Score<{1};", lvlNow, newScore);
				dbCmd.CommandText = sqlQuery;
				dbCmd.ExecuteScalar ();
				dbConnection.Close ();
			}
		}
	}

	void checkScore(){
		if (score==mx) {
			print ("WIN");
			Text2.text = "You win!";
			PauseMenuOpen ();
			setScore (score*100);
		} else {
			//	If the spring had been destroyed (indicating we have launched the projectile) and our projectile's velocity is below the threshold...
			if (trial==tmx) {
				Text2.text = "You lose!";
				setScore (score*100);
				PauseMenuOpen ();
			}
		}
	}

	public void reseter(){
		Application.LoadLevel (Application.loadedLevel);
	}
}
