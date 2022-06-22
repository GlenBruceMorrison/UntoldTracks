using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UntoldTracks.Managers;
using UntoldTracks.Models;

public class DebugCanvas : MonoBehaviour
{
    private bool _active = false;
    
    public TMP_InputField inputField;
    public TMP_Text suggestiveText;

    public string Input
    {
        get => inputField.text;
        set => inputField.text = value;
    }

    public string Suggest
    {
        get => suggestiveText.text;
        set => suggestiveText.text = value;
    }

    private void GainFocus()
    {
        _active = true;
        Debug.Log("GainFocus");
        GameManager.Instance.LocalPlayer.FirstPersonController.Freeze();
        inputField.Select();
        inputField.ActivateInputField();
    }

    private void LooseFocus()
    {
        _active = false;
        Debug.Log("LooseFocus");
        GameManager.Instance.LocalPlayer.FirstPersonController.UnFreeze();
        inputField.text = "";
        inputField.DeactivateInputField();
    }

    private void Awake()
    {
        ItemCommands();
        GameCommands();
        TrainCommands();
        IslandCommands();
    }

    private void IslandCommands()
    {

    }

    private void TrainCommands()
    {
        var root = new DebugCommandBase("train", "train", "train");

        root.AddSubCommand(new DebugCommand("start", "start", "", () =>
        {
            GameManager.Instance.TrainManager.Train.Move();
        }));

        root.AddSubCommand(new DebugCommand("stop", "stop", "", () =>
        {
            GameManager.Instance.TrainManager.Train.Brake();
        }));

        root.AddSubCommand(new DebugCommand<int>("speed", "speed", "", (x) =>
        {
            GameManager.Instance.TrainManager.Train.CurrentSpeed = x;
        }));

        root.AddSubCommand(new DebugCommand<int>("speed_max", "speed_max", "", (x) =>
        {
            GameManager.Instance.TrainManager.Train.MaxSpeed = x;
        }));

        root.AddSubCommand(new DebugCommand("add_carriage", "", "", () =>
        {
            //
        }));
    }

    private void GameCommands()
    {
        var root = new DebugCommandBase("game", "game", "game");

        root.AddSubCommand(new DebugCommand("save", "save", "save", () =>
        {
            Debug.Log("saving game");
            GameManager.Instance.SaveGameData();
        }));

        root.AddSubCommand(new DebugCommand("reset", "reset", "reset", () =>
        {
            GameManager.Instance.Reset();
        }));
    }

    public void ItemCommands()
    {
        var items = ResourceService.Instance.GetAll<ItemModel>();

        var root = new DebugCommandBase("item", "item", "item");

        var itemGiveCommand = new DebugCommandBase("give", "give", "give");
        root.AddSubCommand(itemGiveCommand);


        foreach (var itm in items)
        {
            var command = new DebugCommand<int>($"{itm.name.ToLower()}", "", $"{itm.name.ToLower()}", (amount) =>
            {
                GameManager.Instance.LocalPlayer.InventoryController.Inventory.Give(new UntoldTracks.InventorySystem.ItemContainer(itm, amount));
            });

            itemGiveCommand.AddSubCommand(command);
        }
    }

    public void HandleInputFieldUpdated()
    {
        foreach (var command in DebugCommandBase.DebugCommandsFull.OrderByDescending(x => x.Key.Length).ToList())
        {
            if (command.Value.FullPath.StartsWith(Input))
            {
                Suggest = command.Value.FullPath;
                return;
            }
        }

        Suggest = String.Empty;
    }

    public void CommandEval(DebugCommandBase command, string[] keywords, int index)
    {
        LooseFocus();

        if (keywords.Length == index)
        {
            var subWords = keywords[index - 1].Split(' ');

            switch (command)
            {
                case DebugCommand<string> dcString:
                    {
                        dcString.Invoke(subWords[1]);
                        break;
                    }
                case DebugCommand<int> dcInt:
                    {
                        dcInt.Invoke(int.Parse(subWords.Length < 2 ? "1" : subWords[1]));
                        break;
                    }
                case DebugCommand<Vector3> dcVector:
                    {

                        break;
                    }
                case DebugCommand dc:
                    {
                        Debug.Log("invoke");
                        dc.Invoke();
                        break;
                    }
            }
            return;
        }

        var commandText = keywords[index];

        if (command.HasSubCommands)
        {
            var matchedCommand = command.GetSubCommand(commandText.Split(' ')[0]);

            if (matchedCommand != null)
            {
                Debug.Log("found sub keyword " + matchedCommand.Id);
                CommandEval(matchedCommand, keywords, index+1);
            }
        }
    }

    public void HandleFieldExecute()
    {
        var inputParts = Input.Split(DebugCommandBase.SEPERATOR);
        var mainKeyword = inputParts[0];

        if (DebugCommandBase.DebugCommands.TryGetValue(mainKeyword.ToLower(), out DebugCommandBase command))
        {
            Debug.Log("found main keyword " + mainKeyword);
            CommandEval(command, inputParts, 1);
        }
    }

    private void Update()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.RightArrow))
        {
            Input = Suggest;
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.Return))
        {
            if (!_active)
            {
                GainFocus();
            }
        }
    }

    public class DebugCommandBase
    {
        public const char SEPERATOR = '/';

        private string _fullPath;

        private string _id = "";
        private string _description = "";
        private string _format = "";
        private string _successMessage = "";
        private string _parentCommand = "";
        private List<DebugCommandBase> _subCommands = new();

        public string Id => _id;
        public string Description => _description;
        public string Format => _format;
        public string MessageSuccess => _successMessage;
        public string FullPath => _fullPath;

        public static Dictionary<string, DebugCommandBase> DebugCommands;
        public static Dictionary<string, DebugCommandBase> DebugCommandsFull;

        public bool HasSubCommands => _subCommands.Count > 0;

        public DebugCommandBase(string id, string description = "", string format = "", string succesMessage="")
        {
            _id = id;
            _description = description;
            _format = format;
            _successMessage = succesMessage;

            _fullPath = id;

            if (DebugCommands == null)
            {
                DebugCommands = new Dictionary<string, DebugCommandBase>();
                DebugCommandsFull = new Dictionary<string, DebugCommandBase>();
            }

            string mainKeyword = format.Split(' ')[0];

            DebugCommands[mainKeyword] = this;
            DebugCommandsFull[_fullPath] = this;
        }

        public void AddSubCommand(DebugCommandBase command)
        {
            _subCommands.Add(command);
            command.SetParentCommand(this);
            Debug.Log(command._fullPath);
        }

        public DebugCommandBase GetSubCommand(string id)
        {
            return _subCommands.FirstOrDefault(x => x.Id == id);
        }

        public void SetParentCommand(DebugCommandBase parent)
        {
            _fullPath = parent._fullPath + SEPERATOR + _fullPath;
        }
    }

    public class DebugCommand : DebugCommandBase
    {
        private readonly Action _action;

        public DebugCommand(string id, string description, string format, Action action)
            : base(id, description, format)
        {
            _action = action;
        }

        public void Invoke()
        {
            _action.Invoke();
        }
    }
    
    public class DebugCommand<T> : DebugCommandBase
    {
        private readonly Action<T> _action;

        public DebugCommand(string id, string description, string format, Action<T> action)
            : base(id, description, format)
        {
            _action = action;
        }

        public void Invoke(T val)
        {
            _action.Invoke(val);
        }
    }
}
