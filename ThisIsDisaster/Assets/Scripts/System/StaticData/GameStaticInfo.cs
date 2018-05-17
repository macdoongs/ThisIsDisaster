using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameStaticInfo
{
    /// <summary>
    /// Value of  x / y (2)
    /// </summary>
    public const float VerticalRatio = 2f;

    /// <summary>
    /// Value of  y / x (0.5)
    /// </summary>
    public const float HorizontalRatio = 0.5f;

    /// <summary>
    /// Value of  z / x (0.2)
    /// </summary>
    public const float CrossRatio = 0.2f;

    /// <summary>
    /// 게임에서 사용되는 타일의 가로 길이
    /// </summary>
    public const float TileWidth = 1f;

    /// <summary>
    /// 게임에서 사용되는 타일의 가로 길이의 절반
    /// </summary>
    public const float TileWidth_Half = 0.5f;

    /// <summary>
    /// 게임에서 사용되는 타일의 세로 높이
    /// </summary>
    public const float TileHeight = 0.5f;
    /// <summary>
    /// 게임에서 사용되는 타일의 세로 높이의 절반
    /// </summary>
    public const float TileHeight_Half = 0.25f;

    /// <summary>
    /// 게임에서 사용되는 타일의 y 축 기준점->플레이어 유닛, NPC등에 사용될 것
    /// </summary>
    public const float ZeroHeight = 0.35f;

    /// <summary>
    /// 이동속도 배수
    /// </summary>
    public const float GameSpeedFactor = 0.01f;


}