using Firebase.Database;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    void Start()
    {
        var justin = new Player();
        justin.id = "S10270417C";
        justin.name = "Keanen Lim Xi En";
        justin.email = "S10270417C@connect.np.edu.sg";
        justin.age = 20;
        justin.phoneNumber = 93820569;
        justin.gender = "Male";

        var json = JsonUtility.ToJson(justin);

        Debug.Log(json);
        var db = FirebaseDatabase.DefaultInstance.RootReference;
        db.Child("Player").SetRawJsonValueAsync(json);
    }

}