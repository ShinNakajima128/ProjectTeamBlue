using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundTag
{
	//BGM
	private const string  _bgmFound = "発見";
	private const string  _bgmStage1 = "Stage1";
	private const string _bgmLobby = "ロビー";

	//SE
	private const string _seCursorMove = "CursorMove";
	private const string _seCancel = "Cancel";
	private const string _seView = "View";
	private const string _seCompleteMission = "CompleteMission";
	private const string _seExplosion = "Explosion";
	private const string _seAttack = "Attack";

	

	//BGM
	public static string BGMFound => _bgmFound;
    public static string BGMStage1 => _bgmStage1;
    public static string BGMLobby => _bgmLobby;

	//SE
	public static string SECursorMove => _seCursorMove;
	public static string SECancel => _seCancel;
	public static string SEView => _seView;
	public static string SECompleteMission => _seCompleteMission;
	public static string SEExplosion => _seExplosion;
	public static string SEAttack => _seAttack;
}
