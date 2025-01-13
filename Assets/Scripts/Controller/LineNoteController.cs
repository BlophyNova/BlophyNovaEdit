using Data.ChartData;
using Data.Interface;
using Manager;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UtilityCode.Algorithm;

namespace Controller
{
    public class LineNoteController : MonoBehaviour, IRefresh
    {
        [FormerlySerializedAs("freeBox_NoteParsent")]
        [Tooltip("如果是自由框，那就用这个作为音符的爸爸")]
        public Transform freeBoxNoteParsent;

        public DecideLineController decideLineController; //判定线控制

        public List<NoteController> ariseOnlineNotes = new(); //判定线上方已经出现的音符列表,

        [FormerlySerializedAs("endTime_ariseOnlineNotes")]
        public List<NoteController> endTimeAriseOnlineNotes = new(); //判定线上方已经出现的音符列表,按照EndTime排序

        public List<NoteController> ariseOfflineNotes = new(); //判定线下方已经出现的音符列表,

        [FormerlySerializedAs("endTime_ariseOfflineNotes")]
        public List<NoteController> endTimeAriseOfflineNotes = new(); //判定线下方已经出现的音符列表,按照EndTime排序

        public int lastOnlineIndex; //上次召唤到Note[]列表的什么位置了，从上次的位置继续
        public int lastOfflineIndex; //上次召唤到Note[]列表的什么位置了，从上次的位置继续

        public int movedOnlineNotesCount;
        public int movedOfflineNotesCount;

        public int decideLineOnlineNoteCount = -1;
        //public int decideLineOfflineNoteCount = -1;
        private void Update()
        {
            if (decideLineController.ThisLine.OnlineNotesLength > 0)
            {
                Find_Get_Update_PassHit_Return(decideLineController.ThisLine.onlineNotes, ref lastOnlineIndex,
                    ariseOnlineNotes, endTimeAriseOnlineNotes, true);
            }

            if (decideLineController.ThisLine.OfflineNotesLength > 0)
            {
                Find_Get_Update_PassHit_Return(decideLineController.ThisLine.offlineNotes, ref lastOfflineIndex,
                    ariseOfflineNotes, endTimeAriseOfflineNotes, false);
            }

            if(decideLineOnlineNoteCount != decideLineController.ThisLine.onlineNotes.Count)
            {
                decideLineOnlineNoteCount = decideLineController.ThisLine.onlineNotes.Count;
                ResetLineNoteState(ref lastOnlineIndex, ariseOnlineNotes, endTimeAriseOnlineNotes, decideLineController, decideLineController.thisLine.onlineNotes, true);
            }
            //if (decideLineOfflineNoteCount != decideLineController.ThisLine.offlineNotes.Count)
            //{
            //    decideLineOfflineNoteCount = decideLineController.ThisLine.offlineNotes.Count;
            //    ResetLineNoteState(ref lastOfflineIndex, ariseOfflineNotes, endTimeAriseOfflineNotes, decideLineController, decideLineController.thisLine.offlineNotes, false);
            //}
        }

        public void Refresh()
        {
            ResetLineNoteState(ref lastOnlineIndex, ariseOnlineNotes, endTimeAriseOnlineNotes, decideLineController, decideLineController.thisLine.onlineNotes, true);
            ResetLineNoteState(ref lastOfflineIndex, ariseOfflineNotes, endTimeAriseOfflineNotes, decideLineController, decideLineController.thisLine.offlineNotes, false);
        }

        private void Find_Get_Update_PassHit_Return(List<Note> lineNotes, ref int lastIndex,
            List<NoteController> ariseNotes, List<NoteController> endTimeAriseTime, bool isOnlineNotes)
        {
            FindAndGetNotes(lineNotes, ref lastIndex, ariseNotes, endTimeAriseTime,
                isOnlineNotes); //寻找这一时刻，在判定线下方需要生成的音符
            if (ariseNotes.Count <= 0)
            {
                return;
            }

            UpdateNotes(ariseNotes); //音符出现后每一帧调用
            FindPassHitTimeNotes(ariseNotes); //音符过了打击时间但是没有Miss掉的这个期间每一帧调用
            FindAndReturnNotes(ariseNotes, endTimeAriseTime, isOnlineNotes); //寻找这一时刻，在判定线下方需要回收的Miss掉的音符
        }

        /// <summary>
        ///     音符出现后每一帧调用
        /// </summary>
        /// <param name="ariseNotes">需要调用的列表</param>
        private void UpdateNotes(List<NoteController> ariseNotes)
        {
            int ariseNotesCount = ariseNotes.Count;
            for (int i = 0; i < ariseNotesCount; i++) //循环调用
            {
                ariseNotes[i].NoteHoldArise(); //调用
            }
        }

        /// <summary>
        ///     音符过了打击时间但是没有Miss掉的这个期间每一帧调用
        /// </summary>
        /// <param name="ariseNotes">需要调用的列表</param>
        private void FindPassHitTimeNotes(List<NoteController> ariseNotes)
        {
            int index = Algorithm.BinarySearch(ariseNotes,
                m => m.thisNote.hitTime < ProgressManager.Instance.CurrentTime + .00001,
                false); //寻找音符过了打击时间但是没有Miss掉的音符
            for (int i = index - 1; i >= 0; i--) //循环遍历所有找到的音符
            {
                ariseNotes[i].PassHitTime(ProgressManager.Instance.CurrentTime); //吧音符单独拿出来
            }
        }

        /// <summary>
        ///     寻找这一时刻，在判定线需要生成的音符
        /// </summary>
        /// <param name="notes">音符列表</param>
        /// <param name="lastIndex">上次在什么地方结束，这次就从什么地方继续</param>
        /// <param name="arisedNotes">生成后的音符存放点</param>
        /// <param name="isOnlineNote">当前处理的是不是判定线上方的音符，true代表是判定线上方的音符，false代表不是判定线上方的音符</param>
        private void FindAndGetNotes(List<Note> notes, ref int lastIndex, List<NoteController> arisedNotes,
            List<NoteController> endTimeArisedNotes, bool isOnlineNote)
        {
            Vector3 direction = isOnlineNote switch //确定方向，如果是判定线上方，就是正值，如果是判定线下方，就是负值
            {
                true => Vector3.forward,
                false => Vector3.back
            };
            int index = FindNote(notes);
            for (int i = lastIndex; i < index; i++) //i从上次的地方继续，结束索引是寻找到的索引位置
            {
                //遍历所有符合要求的音符
                GetNote(notes, arisedNotes, endTimeArisedNotes, isOnlineNote, direction, i);
            }

            lastIndex = index; //更新暂停位置
        }

        /// <summary>
        ///     寻找音符
        /// </summary>
        /// <param name="notes">寻找当前的需要出现的时间的音符</param>
        /// <returns>返回索引</returns>
        private int FindNote(List<Note> notes)
        {
            return Algorithm.BinarySearch(notes,
                m => m.hitFloorPosition < -decideLineController.onlineNote.localPosition.y + 2.00001f, false);
            //寻找这个时刻需要出现的音符，出现要提前两个单位长度的时间出现
        }

        /// <summary>
        ///     获取到音符
        /// </summary>
        /// <param name="notes">音符源数据列表</param>
        /// <param name="arisedNotes">出现的音符列表</param>
        /// <param name="endTime_arisedNotes">按照endTime排序的出现的音符列表</param>
        /// <param name="isOnlineNote">是否是线上的音符</param>
        /// <param name="direction">音符朝向</param>
        /// <param name="i">当前处于什么循环</param>
        private void GetNote(List<Note> notes, List<NoteController> arisedNotes,
            List<NoteController> endTimeArisedNotes, bool isOnlineNote, Vector3 direction, int i)
        {
            Note note = notes[i]; //拿出当前遍历到的音符
            NoteController noteController = decideLineController.GetNote(note.noteType, isOnlineNote); //从对象池拿出来
            noteController.thisNote = note; //将这个音符的源数据赋值过去
            noteController.isOnlineNote = isOnlineNote; //将这个音符的源数据赋值过去
            noteController.decideLineController = decideLineController; //将这个音符的源数据赋值过去
            noteController.noteCanvas = isOnlineNote switch
            {
                true => decideLineController.onlineNote, //如果是线上音符就赋值onlineNote
                false => decideLineController.offlineNote //如果是线上音符就赋值offlineNote
            };

            noteController.transform.SetLocalPositionAndRotation(
                new Vector2(note.positionX, note.hitFloorPosition * direction.z),
                Quaternion.Euler(isOnlineNote ? Vector3.zero : Vector3.forward * 180));
            noteController.Init(); //执行音符初始化

            AddNote2NoteList(arisedNotes, endTimeArisedNotes, noteController); //添加音符到音符列表
        }

        /// <summary>
        ///     添加音符到音符列表
        /// </summary>
        /// <param name="arisedNotes">按照hitTime排序的列表</param>
        /// <param name="endTime_arisedNotes">按照endTime排序的列表</param>
        /// <param name="note">需要添加的音符</param>
        private static void AddNote2NoteList(List<NoteController> arisedNotes, List<NoteController> endTimeArisedNotes,
            NoteController note)
        {
            int index = Algorithm.BinarySearch(endTimeArisedNotes, m => m.thisNote.EndTime < note.thisNote.EndTime,
                false); //寻找这个音符按照endTime排序的话，因在插在什么位置
            endTimeArisedNotes.Insert(index, note); //插入音符
            arisedNotes.Add(endTimeArisedNotes[index]); //把音符插入到最后正向排序的最后一个元素中
        }

        /// <summary>
        ///     寻找这一时刻，在判定线需要回收的Miss掉的音符
        /// </summary>
        /// <param name="endTime_ariseNotes">已经出现的音符列表存放点</param>
        /// <param name="isOnlineNote">是判定线上方还是下方</param>
        private void FindAndReturnNotes(List<NoteController> ariseNotes, List<NoteController> endTimeAriseNotes,
            bool isOnlineNote)
        {
            int index = FindMissNote(endTimeAriseNotes); //寻找Miss的音符的索引
            for (int i = 0; i < index; i++) //循环遍历所有Miss掉的音符
            {
                NoteController note = endTimeAriseNotes[i]; //吧音符单独拿出来
                ariseNotes.Remove(note); //移除
                endTimeAriseNotes.Remove(note); //移除
                index--; //移除后当前索引--，不然可能会误伤到其他音符
                note.ReturnPool(); //调用音符的返回对象池的回调方法
                switch (isOnlineNote)
                {
                    case true: //如果是判定线上方
                        decideLineController.ReturnNote(note, note.thisNote.noteType, true); //那就返回到判定线上方对应的对象池中去
                        break;
                    case false: //如果是判定线下方
                        decideLineController.ReturnNote(note, note.thisNote.noteType, false); //那就返回到判定线下方对应的对象池中去
                        break;
                }
            }
        }

        /// <summary>
        ///     寻找已经出现的音符中有没有Miss掉的音符
        /// </summary>
        /// <param name="notes">音符列表</param>
        /// <returns>索引</returns>
        private static int FindMissNote(List<NoteController> notes)
        {
            return Algorithm.BinarySearch(notes, m => ProgressManager.Instance.CurrentTime >= m.thisNote.EndTime,
                false);
            //寻找已经出现的音符中有没有Miss掉的音符
        }

        /// <summary>
        ///     看看世界坐标轴的触摸是不是在这根线的判定范围内
        /// </summary>
        /// <param name="currentPosition">当前手指的世界坐标</param>
        /// <returns>返回是或否</returns>
        public bool SpeckleInThisLine(Vector2 currentPosition)
        {
            //float onlineJudge = ValueManager.Instance.onlineJudgeRange;
            //float offlineJudge = ValueManager.Instance.offlineJudgeRange;

            float inThisLine = transform.InverseTransformPoint(currentPosition).y; //将手指的世界坐标转换为局部坐标后的y拿到

            if (inThisLine <= ValueManager.Instance.onlineJudgeRange && //如果y介于ValueManager设定的数值之间
                inThisLine >= ValueManager.Instance.offlineJudgeRange)
            {
                //UIManager.Instance.DebugTextString = $"onlineJudge:{onlineJudge}||offlineJudge:{offlineJudge}||Result:true ||inThisLine:{inThisLine}";
                return true; //返回是
            }

            //UIManager.Instance.DebugTextString = $"onlineJudge:{onlineJudge}||offlineJudge:{offlineJudge}||Result:false||inThisLine:{inThisLine}";
            return false; //返回否
        }

        public void ResetLineNoteState(ref int lastIndex, List<NoteController> ariseLineNotes,
            List<NoteController> endTime_ariseLineNotes, DecideLineController decideLine, List<Note> notes,
            bool isOnlineNote)
        {
            lastIndex = 0;
            for (int i = 0; i < notes.Count; i++)
            {
                if (notes[i].hitTime < ProgressManager.Instance.CurrentTime)
                {
                    lastIndex++;
                }
                else
                {
                    break;
                }
            }

            for (int i = 0; i < ariseLineNotes.Count; i++)
            {
                NoteController note = ariseLineNotes[i];
                decideLine.ReturnNote(note, note.thisNote.noteType, isOnlineNote);
            }

            ariseLineNotes.Clear();
            endTime_ariseLineNotes.Clear();
        }
    }
}