
                _serverthread.Stop();
            }
        };
        _serverthread.UpdateEndStrategy(_endAction);
        _serverthread.UpdateBehaviour(softAction);
    }
}
        Action softAct = () =>
        {
            if (q.Count != 0)
            {
                var cmd = q.Take();
                try
                {
                    cmd.Execute();
                }
                catch (Exception e)
                {
                    IoC.Resolve<ICommand>("Exception.Handler", cmd, e).Execute();
                }
            }
            else
            {
                _st.Stop();
            }
        };
        _st.UpdateEndStrategy(_endAction);
        _st.UpdateBehaviour(softAct);
    }
}