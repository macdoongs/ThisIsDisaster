using System.Collections; 
using System.Collections.Generic; 
using UnityEngine; 
using UnityEngine.UI; 
using Firebase; 
using Firebase.Auth; 
using Firebase.Unity.Editor; 
using Facebook.Unity; 
using GooglePlayGames; 
using GooglePlayGames.BasicApi; 
using UnityEngine.SocialPlatforms; 
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class FirebaseManager : MonoBehaviour
{


#if true

    // 이메일 InputField 
    [SerializeField] 
	InputField emailInput; 
	// 비밀번호 InputField 
	[SerializeField] 
	InputField passInput; 
	// 결과를 알려줄 텍스트 
	[SerializeField] 
	Text resultText; 

	// 인증을 관리할 객체 
	Firebase.Auth.FirebaseAuth auth; 
	/** 사용자 */ 
	Firebase.Auth.FirebaseUser user; 

	string displayName; 
	string emailAddress; 
	string photoUrl; 
	string authCode; 

	// Use this for initialization 
	void Awake () 
	{ 

		// facebook sdk 초기화 
		if (!FB.IsInitialized) { 
			FB.Init(FacebookInitCallBack, OnHideUnity); 
		} 

		// 초기화 
		InitializeFirebase(); 

		InitalizeGooglePlay();
	} 

	void InitializeFirebase() { 
		auth = Firebase.Auth.FirebaseAuth.DefaultInstance; 
		auth.StateChanged += AuthStateChanged; 
	} 

	void InitalizeGooglePlay() {
		PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder() 
			.RequestServerAuthCode(false /* Don't force refresh */)
			.RequestIdToken()
			.Build(); 

		PlayGamesPlatform.InitializeInstance(config); 
		PlayGamesPlatform.DebugLogEnabled = true; 
		PlayGamesPlatform.Activate(); 
	}

	/** firebase 앱 내에 가입 여부를 체크한다. */ 
	private bool SignedInFirebase { 
		get { 
			return user != auth.CurrentUser && auth.CurrentUser != null; 
		} 
	} 

	/** 상태변화 추적 */ 
	void AuthStateChanged(object sender, System.EventArgs eventArgs) { 
		if (auth.CurrentUser != user) { 

			if (!SignedInFirebase && user != null) { 
				Debug.LogFormat("Signed out {0}", user.UserId); 
			} 
			user = auth.CurrentUser; 
			if (SignedInFirebase) { 
				Log(string.Format("Signed in {0}", user.UserId)); 
				displayName = user.DisplayName ?? ""; 
				emailAddress = user.Email ?? ""; 
				Log(string.Format("Signed in {0} _ {1}", displayName, emailAddress)); 

				LoadingSceneManager.LoadScene("Lobby Scene");
			} 
		} 
	} 


#region FACEBOOK 로그인 
	/** Facebook 초기화 콜백 */ 
	void FacebookInitCallBack() { 
		if (FB.IsInitialized) { 
			Log("Successed to Initalize the Facebook SDK"); 
			FB.ActivateApp(); 
		} else { 
			Log("Failed to Initalize the Facebook SDK"); 
		} 
	} 

	/** Facebook 로그인이 활성화되는 경우 호출 */ 
	void OnHideUnity(bool isGameShown) { 
		if (!isGameShown) { 
			// 게임 일시 중지 
			Time.timeScale = 0; 
		} else { 
			// 게임 재시작 
			Time.timeScale = 1; 
		} 
	} 

	/** 페이스북 로그인 요청(버튼과 연결) */ 
	public void facebookLogin() { 
		var param = new List<string>() { "public_profile", "email" }; 
		FB.LogInWithReadPermissions(param, FacebookAuthCallback); 
	} 

	/** 페이스북 로그인 결과 콜백 */ 
	void FacebookAuthCallback(ILoginResult result) { 
		if (result.Error != null) { 
			Log(string.Format("Facebook Auth Error: {0}", result.Error)); 
			return; 
		} 
		if (FB.IsLoggedIn) { 
			var accessToken = AccessToken.CurrentAccessToken; 
			Log(string.Format("Facebook access token: {0}", accessToken.TokenString)); 

			// 이미 firebase에 account 등록이 되었는지 확인 
			if (SignedInFirebase) { 
				linkFacebookAccount(accessToken); 
			} else { 
				// firebase facebook 로그인 연결 호출 부분 
				registerFacebookAccountToFirebase(accessToken); 
			} 

			LoadingSceneManager.LoadScene("Lobby Scene");

		} else { 
			Log("User cancelled login"); 
		} 
	} 

	/** Facebook access token으로 Firebase 등록 요청 */ 
	void registerFacebookAccountToFirebase(AccessToken accessToken) { 
		Credential credential = FacebookAuthProvider.GetCredential(accessToken.TokenString); 

		auth 
			.SignInWithCredentialAsync(credential) 
			.ContinueWith(task => { 
				if (task.IsCanceled) { 
					Log("SignInWithCredentialAsync was canceled."); 
					return; 
				} 
				if (task.IsFaulted) { 
					Log("SignInWithCredentialAsync encountered an error: " + task.Exception); 
					return; 
				} 

				user = task.Result; 
				Log(string.Format("User signed in successfully: {0} ({1})", 
					user.DisplayName, user.UserId)); 
			}); 
	} 

	/** Firebase에 등록된 account를 보유했을 때 새로운 인증을 연결한다. */ 
	void linkFacebookAccount(AccessToken accessToken) { 
		Credential credential = FacebookAuthProvider.GetCredential(accessToken.TokenString); 

		auth.CurrentUser 
			.LinkWithCredentialAsync(credential) 
			.ContinueWith(task => { 
				if (task.IsCanceled) { 
					Log("LinkWithCredentialAsync was canceled."); 
					return; 
				} 
				if (task.IsFaulted) { 
					Log("LinkWithCredentialAsync encountered an error: " + task.Exception); 
					return; 
				} 

				user = task.Result; 
				Log(string.Format("Credentials successfully linked to Firebase user: {0} ({1})", 
					user.DisplayName, user.UserId)); 
				
			}); 
	} 
#endregion

	public void GooglePlayLogin() 
	{ 

		Social.localUser.Authenticate((success, errorMessage) => 
			{ 
				if (success) 
				{ 
					resultText.text = string.Format("Google Play login - {0}:{1}", success, Social.localUser.userName); 
					StartCoroutine(coLogin()); 
				} 
				else 
				{ 

					Debug.Log("google " + errorMessage); 
					return ; 
				} 

				Debug.Log("google " + success); 
			}); 
	} 

	public IEnumerator coLogin() 
	{ 
		resultText.text = string.Format ("\nTry to get Token..."); 
		while (System.String.IsNullOrEmpty (((PlayGamesLocalUser)Social.localUser).GetIdToken ())) 
			yield return null; 

		string idToken = ((PlayGamesLocalUser)Social.localUser).GetIdToken(); 

		resultText.text = string.Format ("\nToken: {0}", idToken); 
		Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance; 

		Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential (idToken, null); 
		auth.SignInWithCredentialAsync (credential).ContinueWith ( 
			task => { 
				if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted) { 
					Firebase.Auth.FirebaseUser newUser = task.Result; 
					resultText.text = string.Format ("FirebaseUser:{0}\nEmail:{1}", newUser.UserId, newUser.Email); 
				} 
			}); 
	} 

	// 회원가입 버튼을 눌렀을 때 작동할 함수 
	public void SignUp() 
	{ 
		// 회원가입 버튼은 인풋 필드가 비어있지 않을 때 작동한다. 
		if(emailInput.text.Length != 0 && passInput.text.Length != 0) 
		{ 
			auth.CreateUserWithEmailAndPasswordAsync(emailInput.text, passInput.text).ContinueWith( 
				task => 
				{ 
					if(!task.IsCanceled && !task.IsFaulted) 
					{ 
						resultText.text = "회원가입 성공"; 
					} 
					else 
					{ 
						resultText.text = "회원가입 실패"; 
					} 
				}); 
		} 
	} 

	// 로그인 버튼을 눌렀을 때 작동할 함수 
	public void SignIn() 
	{ 
		// 로그인 버튼은 인풋 필드가 비어있지 않을 때 작동한다. 
		if (emailInput.text.Length != 0 && passInput.text.Length != 0) 
		{ 
			auth.SignInWithEmailAndPasswordAsync(emailInput.text, passInput.text).ContinueWith( 
				task => 
				{ 
					if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted) 
					{ 
						Firebase.Auth.FirebaseUser newUser = task.Result; 
						resultText.text = "로그인 성공"; 

						LoadingSceneManager.LoadScene("Lobby Scene");
					} 
					else 
					{ 
						resultText.text = "로그인 실패"; 
					} 
				}); 
		} 
	} 

	void Log(string logText) 
	{ 
		resultText.text += (logText + "\n"); 
		Debug.Log(logText); 
	} 
#endif
}