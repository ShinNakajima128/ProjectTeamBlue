using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundTag
{
	//BGM
	private const string  _bgm_Found = "発見";
	private const string  _bgm_Stage1 = "Stage1";
	private const string _bgm_Lobby = "ロビー";
	private const string _bgm_Result = "リザルト";

	//SE
	private const string _se_CursorMove = "CursorMove";
	private const string _se_Cancel = "Cancel";
	private const string _se_View = "View";
	private const string _se_CompleteMainMission = "CompleteMainMission";
	private const string _se_CompleteSubMission = "CompleteSubMission";
	private const string _se_Explosion = "Explosion";
	private const string _se_Attack = "Attack";
	private const string _se_ScoreView = "ScoreView";
	

	//BGM
	public static string BGM_Found => _bgm_Found;
    public static string BGM_Stage1 => _bgm_Stage1;
    public static string BGM_Lobby => _bgm_Lobby;
	public static string BGM_Result => _bgm_Result;

	//SE
	public static string SE_CursorMove => _se_CursorMove;
	public static string SE_Cancel => _se_Cancel;
	public static string SE_View => _se_View;
	public static string SE_CompleteMainMission => _se_CompleteMainMission;
	public static string SE_CompleteSubMission => _se_CompleteSubMission;
	public static string SE_Explosion => _se_Explosion;
	public static string SE_Attack => _se_Attack;
	public static string SE_ScoreView => _se_ScoreView;
}
