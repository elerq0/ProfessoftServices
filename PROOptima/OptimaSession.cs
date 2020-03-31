using System;

namespace PROOptima
{
    class OptimaSession
    {
        private CDNBase.ILogin login = null;
        public CDNBase.AdoSession session = null;
        private readonly int limit = 3;
        private int counter;
        private Boolean sessionRefresh;


        public OptimaSession(CDNBase.ILogin login, Boolean sessionRefresh)
        {
            this.login = login;
            counter = 0;
            this.sessionRefresh = sessionRefresh;

            session = CreateNewSession();
        }


        public void Save(Boolean refresh)
        {
            session.Save();

            if (sessionRefresh)
            {
                if (counter >= limit && refresh)
                    ForceSessionRenew();
                else
                    counter += 1;
            }
        }

        private CDNBase.AdoSession CreateNewSession()
        {
            CDNBase.AdoSession newSession;
            newSession = login.CreateSession();

            if (newSession.Connection.State == 0)
                newSession.Connection.Open();

            if (session != null)
                session.Login.PushBackConnection(newSession.Connection);

            object o = new object();

            newSession.Connection.Execute(
                "SET ANSI_NULLS ON; SET ANSI_PADDING ON; SET ANSI_WARNINGS ON; " +
                "SET ARITHABORT ON; SET CONCAT_NULL_YIELDS_NULL ON; SET QUOTED_IDENTIFIER ON; " +
                "SET NUMERIC_ROUNDABORT OFF", out o);

            return newSession;
        }

        private void CloseSession()
        {
            if (session.Connection.State == 1)
                session.Connection.Close();
        }

        public void ForceSessionRenew()
        {
            CloseSession();
            session = CreateNewSession();
            counter = 0;
        }
    }
}
