using System.Collections.Generic;
using System.Text;
using UnityEngine;

internal class FormattedLabel : IHyperlinkCallback
{
	private enum VerticalAlignment
	{
		Default,
		Bottom
	}

	private class TextFormatter
	{
		private const string _lineHeightCommand = "[LH &]";

		private float _width;

		private List<string> _lines;

		private GUIStyle _guiStyle;

		private StringBuilder _line;

		private float _lineLength;

		private float _lineHeight;

		public TextFormatter(float width, string text)
		{
			_width = width;
			_lines = new List<string>();
			format(text);
		}

		public List<string> getLines()
		{
			return _lines;
		}

		private void format(string text)
		{
			if (text == null)
			{
				Logger.traceError("[TextFormatter::format] - text is null");
				return;
			}
			_guiStyle = new GUIStyle();
			_line = new StringBuilder();
			addLineHeight(realHeight: false);
			_lineLength = 0f;
			_lineHeight = 0f;
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < text.Length; i++)
			{
				int num;
				if (text[i] == '\\' && text.Length > i + 1 && text[i + 1] == '\\')
				{
					stringBuilder.Append("\\");
					i++;
				}
				else if (text[i] == '\n')
				{
					addWordToLine(stringBuilder.ToString());
					createNewLine();
					stringBuilder.Length = 0;
				}
				else if (text[i] == ' ' && stringBuilder.Length != 0)
				{
					addWordToLine(stringBuilder.ToString());
					stringBuilder.Length = 0;
					stringBuilder.Append(' ');
				}
				else if (text[i] == '[' && text.Length > i + 1 && text[i + 1] == '[')
				{
					stringBuilder.Append("[[");
					i++;
				}
				else if (text[i] == '[' && text.Length > i + 1 && (num = text.IndexOf(']', i)) != -1)
				{
					addWordToLine(stringBuilder.ToString());
					stringBuilder.Length = 0;
					string text2 = text.Substring(i + 1, num - i - 1);
					i += text2.Length + 1;
					string[] array = text2.Split(' ');
					for (int j = 0; j < array.Length; j++)
					{
						switch (array[j].ToUpper())
						{
						case "BC":
						case "BACKCOLOR":
							if (array.Length > j + 1)
							{
								j++;
									Color c;
								if (array[j] == "?" || HexUtil.HexToColor(array[j], out c))
								{
									addCommandToLine("BC " + array[j]);
								}
								else
								{
									Debug.LogError("The 'BackColor' command requires a color parameter of RRGGBBAA or '?'.");
								}
							}
							else
							{
								Debug.LogError("The 'BackColor' command requires a color parameter of RRGGBBAA or '?'.");
							}
							break;
						case "C":
						case "COLOR":
							if (array.Length > j + 1)
							{
								j++;
									Color c;
								if (array[j] == "?" || HexUtil.HexToColor(array[j], out c))
								{
									addCommandToLine("C " + array[j]);
								}
								else
								{
									Debug.LogError("The 'color' command requires a color parameter of RRGGBBAA or '?'.");
								}
							}
							else
							{
								Debug.LogError("The 'color' command requires a color parameter of RRGGBBAA:\n" + text);
							}
							break;
						case "F":
						case "FONT":
							if (array.Length > j + 1)
							{
								j++;
								Font font = (Font)Resources.Load("Fonts/" + array[j]);
								if (font == null)
								{
									Debug.LogError("The font '" + array[j] + "' does not exist within Assets/Resources/Fonts/");
								}
								else
								{
									_guiStyle.font = font;
									addCommandToLine("F " + array[j]);
								}
								if (array.Length > j + 1)
								{
									j++;
										int result;
									if (int.TryParse(array[j], out result))
									{
										addCommandToLine("FS " + array[j]);
										_guiStyle.fontSize = result;
									}
									else
									{
										Debug.LogError("The font size '" + array[j] + "' is not a valid integer");
									}
								}
							}
							else
							{
								Debug.LogError("The 'font' command requires a font name parameter and an optional font size parameter.");
							}
							break;
						case "FA":
						case "FONTATTRIBUTE":
							if (array.Length > j + 1)
							{
								j++;
								string text5;
								switch (array[j].ToUpper())
								{
								case "U":
								case "UNDERLINE":
									text5 = "U";
									break;
								case "-U":
								case "-UNDERLINE":
									text5 = "-U";
									break;
								case "S":
								case "STRIKETHROUGH":
									text5 = "S";
									break;
								case "-S":
								case "-STRIKETHROUGH":
									text5 = "-S";
									break;
								default:
									text5 = string.Empty;
									Debug.LogError("The 'font attribute' command requires a font parameter of U (underline on), -U (underline off), S (strikethrough on) or -S (strikethrough off).");
									break;
								}
								if (text5.Length != 0)
								{
									addCommandToLine("FA " + text5);
								}
							}
							else
							{
								Debug.LogError("The 'font attribute' command requires a font parameter of U (underline on), -U (underline off), S (strikethrough on) or -S (strikethrough off).");
							}
							break;
						case "FS":
						case "FONTSIZE":
							if (array.Length > j + 1)
							{
								j++;
									int result3;
								if (int.TryParse(array[j], out result3))
								{
									addCommandToLine("FS " + array[j]);
									_guiStyle.fontSize = result3;
								}
								else
								{
									Debug.LogError("The font size '" + array[j] + "' is not a valid integer");
								}
							}
							else
							{
								Debug.LogError("The 'font size' command requires a font size parameter.");
							}
							break;
						case "H":
						case "HYPERLINK":
							if (array.Length > j + 1)
							{
								j++;
								addCommandToLine("H " + array[j]);
							}
							else
							{
								Debug.LogError("The 'hyperlink' command requires an hyperlink id parameter.");
							}
							break;
						case "-H":
						case "-HYPERLINK":
							addCommandToLine("-H");
							break;
						case "HA":
						case "HALIGN":
							if (array.Length > j + 1)
							{
								j++;
								string text4;
								switch (array[j].ToUpper())
								{
								case "L":
								case "LEFT":
									text4 = "L";
									break;
								case "R":
								case "RIGHT":
									text4 = "R";
									break;
								case "C":
								case "CENTER":
									text4 = "C";
									break;
								default:
									text4 = string.Empty;
									Debug.LogError("The 'HAlign' command requires an alignment parameter of L (left), R (right), or C (center).");
									break;
								}
								if (text4.Length != 0)
								{
									addCommandToLine("HA " + text4);
								}
							}
							else
							{
								Debug.LogError("The 'HAlign' command requires an alignment parameter of L (left), R (right), or C (center).");
							}
							break;
						case "S":
						case "SPACE":
							if (array.Length > j + 1)
							{
								j++;
									int result2;
								if (int.TryParse(array[j], out result2))
								{
									addCommandToLine("S " + array[j]);
									_lineLength += result2;
								}
								else
								{
									Debug.LogError("The space size '" + array[j] + "' is not a valid integer");
								}
							}
							else
							{
								Debug.LogError("The 'space' command requires a pixel count parameter.");
							}
							break;
						case "VA":
						case "VALIGN":
							if (array.Length > j + 1)
							{
								j++;
								string text3;
								switch (array[j].ToUpper())
								{
								default:
								{
									// TODO: Find out what this is
									int num2 = 1;
									if (num2 == 1)
									{
										text3 = "B";
										break;
									}
									text3 = string.Empty;
									Debug.LogError("The 'VAlign' command requires an alignment parameter of ? (default) or B (bottom).");
									break;
								}
								case "?":
									text3 = "?";
									break;
								}
								if (text3.Length != 0)
								{
									addCommandToLine("VA " + text3);
								}
							}
							else
							{
								Debug.LogError("The 'VAlign' command requires an alignment parameter of ? (default) or B (bottom).");
							}
							break;
						}
					}
				}
				else
				{
					stringBuilder.Append(text[i]);
				}
			}
			addWordToLine(stringBuilder.ToString());
			addLineToLines();
		}

		private bool addWordToLine(string word)
		{
			bool result = false;
			if (word.Length != 0)
			{
				Vector2 vector = _guiStyle.CalcSize(new GUIContent(word));
				float num = (!(word == " ")) ? vector.x : getSpaceWidth();
				if (_lineLength + num > _width)
				{
					createNewLine();
					result = true;
					word = word.TrimStart(' ');
					Vector2 vector2 = _guiStyle.CalcSize(new GUIContent(word));
					num = vector2.x;
				}
				_line.Append(word);
				_lineLength += num;
				_lineHeight = Mathf.Max(_lineHeight, vector.y);
			}
			return result;
		}

		private float getSpaceWidth()
		{
			Vector2 vector = _guiStyle.CalcSize(new GUIContent("**"));
			float x = vector.x;
			Vector2 vector2 = _guiStyle.CalcSize(new GUIContent("* *"));
			float x2 = vector2.x;
			return x2 - x;
		}

		private void addCommandToLine(string command)
		{
			bool flag = command.StartsWith("HA ");
			command = "[" + command + "]";
			if (flag)
			{
				_line.Insert(0, command);
				return;
			}
			int num = _line.Length - _line.ToString().TrimEnd(' ').Length;
			if (num != 0)
			{
				string value = _line.ToString().TrimEnd(' ');
				_line.Length = 0;
				_line.Append(value);
				float num2 = getSpaceWidth() * (float)num;
				command = "[S " + num2 + "]" + command;
				_lineLength += num2;
			}
			_line.Append(command);
		}

		private void addLineToLines()
		{
			if (_line.ToString() == "[LH &]")
			{
				_line.Append(" ");
			}
			addLineHeight(realHeight: true);
			_lines.Add(_line.ToString());
		}

		private void createNewLine()
		{
			addLineToLines();
			_line.Length = 0;
			addLineHeight(realHeight: false);
			_lineLength = 0f;
		}

		private void addLineHeight(bool realHeight)
		{
			if (realHeight)
			{
				string newValue = "[LH &]".Replace("&", _lineHeight.ToString());
				_line.Replace("[LH &]", newValue);
				_lineHeight = 0f;
			}
			else
			{
				_line.Append("[LH &]");
			}
		}
	}

	public enum TestText
	{
		Demo,
		Fireball,
		Hyperlink,
		SpecialText
	}

	private const string HYPERLINK_TAG = "Hyperlink_";

	private List<string> _lines;

	private bool _fontUnderline;

	private bool _fontStrikethrough;

	private Texture2D _backgroundColor;

	private IHyperlinkCallback _hyperlinkCallback;

	private string _lastTooltip = string.Empty;

	private string _createHyperlinkId = string.Empty;

	private string _hoveredHyperlinkId = string.Empty;

	private bool _activatedHyperlink;

	private VerticalAlignment _verticalAlignment;

	private float _lineHeight;

	private Color _defaultColor;

	private Texture2D _defaultBackgroundColor;

	public int mDrawlength = -1;

	public FormattedLabel(float width, string text)
	{
		TextFormatter textFormatter = new TextFormatter(width, text);
		_lines = textFormatter.getLines();
	}

	public List<string> getLines()
	{
		return _lines;
	}

	public void draw()
	{
		TextAlignment textAlignment = TextAlignment.Left;
		GUILayout.BeginVertical();
		GUILayout.BeginHorizontal();
		GUIStyle gUIStyle = new GUIStyle();
		gUIStyle.normal.textColor = GUI.skin.GetStyle("Label").normal.textColor;
		gUIStyle.font = GUI.skin.font;
		gUIStyle.border = new RectOffset(0, 0, 0, 0);
		gUIStyle.contentOffset = new Vector2(0f, 0f);
		gUIStyle.margin = new RectOffset(0, 0, 0, 0);
		gUIStyle.padding = new RectOffset(0, 0, 0, 0);
		_defaultColor = gUIStyle.normal.textColor;
		_defaultBackgroundColor = GUI.skin.GetStyle("Label").normal.background;
		int num = mDrawlength;
		foreach (string line in _lines)
		{
			if (num == 0)
			{
				break;
			}
			int num2 = 0;
			if (line.Length >= 5 && line.StartsWith("[HA "))
			{
				num2 = 6;
				switch (line[4])
				{
				case 'L':
					textAlignment = TextAlignment.Left;
					break;
				case 'R':
					textAlignment = TextAlignment.Right;
					break;
				case 'C':
					textAlignment = TextAlignment.Center;
					break;
				}
			}
			if (textAlignment == TextAlignment.Right || textAlignment == TextAlignment.Center)
			{
				GUILayout.FlexibleSpace();
			}
			while (num2 < line.Length)
			{
				int num3 = line.IndexOf('[', num2);
				if (num3 == num2 && num3 + 1 < line.Length && line[num3 + 1] == '[')
				{
					if (num != -1)
					{
						num--;
					}
					drawText(gUIStyle, line.Substring(num2, 1));
					num2 += 2;
				}
				else if (num3 == num2)
				{
					int num4 = line.IndexOf(']', num3);
					num2 = num4 + 1;
					if (num4 < num3)
					{
						Debug.Log("<< length: " + line.Length + "  command start: " + num3 + " commandEnd: " + num4);
						Debug.Log("line: " + line);
						return;
					}
					string[] array = line.Substring(num3 + 1, num4 - num3 - 1).Split(' ');
					switch (array[0])
					{
					case "BC":
						if (array[1] == "?")
						{
							gUIStyle.normal.background = _defaultBackgroundColor;
						}
						else
						{
								Color color;
							HexUtil.HexToColor(array[1], out color);
							_backgroundColor = new Texture2D(1, 1);
							_backgroundColor.SetPixel(0, 0, color);
							_backgroundColor.wrapMode = TextureWrapMode.Repeat;
							_backgroundColor.Apply();
							gUIStyle.normal.background = _backgroundColor;
						}
						break;
					case "C":
						if (array[1] == "?")
						{
							gUIStyle.normal.textColor = _defaultColor;
						}
						else
						{
								Color color2;
							HexUtil.HexToColor(array[1], out color2);
							gUIStyle.normal.textColor = color2;
						}
						break;
					case "F":
						gUIStyle.font = (Font)Resources.Load("Fonts/" + array[1]);
						break;
					case "FA":
						if (array[1] == "U")
						{
							_fontUnderline = true;
						}
						else if (array[1] == "-U")
						{
							_fontUnderline = false;
						}
						else if (array[1] == "S")
						{
							_fontStrikethrough = true;
						}
						else if (array[1] == "-S")
						{
							_fontStrikethrough = false;
						}
						break;
					case "FS":
						gUIStyle.fontSize = int.Parse(array[1]);
						break;
					case "H":
						_createHyperlinkId = "Hyperlink_" + array[1];
						break;
					case "-H":
						_createHyperlinkId = string.Empty;
						break;
					case "LH":
						_lineHeight = float.Parse(array[1]);
						break;
					case "S":
						GUILayout.Space(float.Parse(array[1]));
						break;
					case "VA":
						switch (array[1])
						{
						case "?":
							_verticalAlignment = VerticalAlignment.Default;
							break;
						case "B":
							_verticalAlignment = VerticalAlignment.Bottom;
							break;
						}
						break;
					}
				}
				else if (num3 == -1)
				{
					string text = line.Substring(num2);
					if (num != -1)
					{
						if (text.Length > num)
						{
							text = text.Substring(0, num);
							num = 0;
						}
						else
						{
							num -= text.Length;
						}
					}
					drawText(gUIStyle, text);
					num2 = line.Length;
				}
				else
				{
					string text2 = line.Substring(num2, num3 - num2);
					if (num != -1)
					{
						if (text2.Length > num)
						{
							text2 = text2.Substring(0, num);
							num = 0;
						}
						else
						{
							num -= text2.Length;
						}
					}
					drawText(gUIStyle, text2);
					num2 = num3;
				}
				if (num == 0)
				{
					break;
				}
			}
			if (textAlignment == TextAlignment.Left || textAlignment == TextAlignment.Center)
			{
				GUILayout.FlexibleSpace();
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
		}
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		handleHyperlink();
	}

	private void handleHyperlink()
	{
		if (Event.current.type == EventType.Repaint && GUI.tooltip != _lastTooltip)
		{
			if (_lastTooltip.StartsWith("Hyperlink_"))
			{
				string hyperlinkId = _lastTooltip.Substring("Hyperlink_".Length);
				onHyperlinkLeave(hyperlinkId);
			}
			if (GUI.tooltip.StartsWith("Hyperlink_"))
			{
				string hyperlinkId2 = GUI.tooltip.Substring("Hyperlink_".Length);
				onHyperlinkEnter(hyperlinkId2);
			}
			_lastTooltip = GUI.tooltip;
		}
		_activatedHyperlink = false;
		if (Event.current != null && Event.current.isMouse && Event.current.type == EventType.MouseUp && Event.current.button == 0 && isHyperlinkHovered())
		{
			onHyperLinkActivated(_hoveredHyperlinkId);
		}
	}

	private void drawText(GUIStyle guiStyle, string text)
	{
		float num;
		Rect lastRect;
		if (_verticalAlignment == VerticalAlignment.Bottom)
		{
			float lineHeight = _lineHeight;
			Vector2 vector = guiStyle.CalcSize(new GUIContent(text));
			num = lineHeight - vector.y + (float)(guiStyle.fontSize - 16) / 4f;
			GUILayout.BeginVertical();
			GUILayout.Label(" ", GUILayout.MinHeight(num), GUILayout.MaxHeight(num));
			GUILayout.Label(new GUIContent(text, _createHyperlinkId), guiStyle);
			lastRect = GUILayoutUtility.GetLastRect();
			GUILayout.EndVertical();
		}
		else
		{
			num = 0f;
			GUILayout.Label(new GUIContent(text, _createHyperlinkId), guiStyle);
			lastRect = GUILayoutUtility.GetLastRect();
		}
		if (Event.current.type == EventType.Repaint)
		{
			if (_fontUnderline)
			{
				Vector2 lineStart = new Vector2(lastRect.x, lastRect.yMin - num + _lineHeight - 3f);
				GuiHelper.DrawLine(lineEnd: new Vector2(lineStart.x + lastRect.width, lineStart.y), lineStart: lineStart, color: guiStyle.normal.textColor);
			}
			if (_fontStrikethrough)
			{
				Vector2 lineStart2 = new Vector2(lastRect.x, lastRect.yMin - num + _lineHeight - _lineHeight / 2f);
				GuiHelper.DrawLine(lineEnd: new Vector2(lineStart2.x + lastRect.width, lineStart2.y), lineStart: lineStart2, color: guiStyle.normal.textColor);
			}
		}
	}

	public void setHyperlinkCallback(IHyperlinkCallback hyperlinkCallback)
	{
		_hyperlinkCallback = hyperlinkCallback;
	}

	public bool isHyperlinkHovered()
	{
		return _hoveredHyperlinkId.Length != 0;
	}

	public string getHoveredHyperlink()
	{
		return _hoveredHyperlinkId;
	}

	public bool isHyperlinkActivated()
	{
		return _activatedHyperlink;
	}

	public string getActivatedHyperlink()
	{
		return (!_activatedHyperlink) ? string.Empty : _hoveredHyperlinkId;
	}

	public void onHyperlinkEnter(string hyperlinkId)
	{
		_hoveredHyperlinkId = hyperlinkId;
		if (_hyperlinkCallback != null)
		{
			_hyperlinkCallback.onHyperlinkEnter(hyperlinkId);
		}
	}

	public void onHyperLinkActivated(string hyperlinkId)
	{
		_activatedHyperlink = true;
		if (_hyperlinkCallback != null)
		{
			_hyperlinkCallback.onHyperLinkActivated(hyperlinkId);
		}
	}

	public void onHyperlinkLeave(string hyperlinkId)
	{
		_hoveredHyperlinkId = string.Empty;
		if (_hyperlinkCallback != null)
		{
			_hyperlinkCallback.onHyperlinkLeave(hyperlinkId);
		}
	}

	public static string GetTestText(TestText testText)
	{
		string result = "FormattedLabel.GetTestText()";
		switch (testText)
		{
		case TestText.Demo:
			result = "This [c 01F573FF]sentence[C FFFFFFFF] is [c FF6666FF]too[C FFFFFFFF] long so it will be [BC 1B07F5FF]split[BC ?] into multiple lines.\nNormal, [F ArialBold]bold, [font ArialItalic]italic, [F Arial][FA u]underline[FA -u], [FA S]strikethrough[FA -s].\n[F Arial 10]10, [F Arial 16]16, [F Arial 24]24, [F Arial 48]48, [F Arial 72]72[F Arial 16]\n[HA L]Left\n[HA C]Center\n[HA R]Right\n[HA L]20 pixels further:[S 20]*\nDefault vertical aligment: [F Arial 10]10, [F Arial 24]24, [F Arial 10]10[FS 16]\n[VA B]Bottom vertical aligment: [F Arial 10]10, [F Arial 24]24, [F Arial 10]10[FS 16][VA ?]\nThis is a [FA U][H hyperlink_value]hyperlink[-H][FA -U].";
			break;
		case TestText.Fireball:
			result = "[HA Center][C FA8C8CFF][FS 24]Fireball[FS 16][color FFFFFFFF]\n\nHurls a ball of fire that [F ArialBold]explodes[F Arial] on [FA U]contact[FA -U] and damages all nearby [FA S]foes [FA -S]enemies.\n\n[VA B][C FF6666FF][F ArialBold 18]8[FS 16][C FFFFFFFF][F Arial] to [C FF6666FF][F ArialBold 18]12[F Arial 16][C FFFFFFFF][F ArialItalic] fire[F Arial] damage[VA ?]";
			break;
		case TestText.Hyperlink:
			result = "This is a hidden [H hidden]hyperlink[-H].\nThis is a visible [FA U][H visible]hyperlink[-H][FA -U].";
			break;
		case TestText.SpecialText:
			result = "Escaped backslash \\\nEscaped bracket [[\nClosing bracket ]\n";
			break;
		default:
			Debug.Log("Invalid index '" + testText.ToString() + "'");
			break;
		}
		return result;
	}
}
