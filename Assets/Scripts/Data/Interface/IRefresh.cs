using System;
using System.Collections.Generic;

namespace Data.Interface
{
    public interface IRefreshUI
    {
        public void RefreshUI();
    }
    public interface IRefreshEdit
    {
        public void RefreshEdit(int lineID, int boxID);
    }
    public interface IRefreshPlayer
    {
        public void RefreshPlayer(int lineID, int boxID);
    }
}