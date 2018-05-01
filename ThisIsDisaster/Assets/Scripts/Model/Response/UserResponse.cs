using System;

[Serializable]
public class User {
	public string nickname;
	public string score;
	public string level;
	public string gold;
}

[Serializable]
public class UserResponse : Response{
	public User result_data;
}
