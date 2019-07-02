﻿namespace Checkers
{
    public class Point
    {
        private int m_X;
        private int m_Y;

        public Point(int i_Y, int i_X)
        {
            m_X = i_X;
            m_Y = i_Y;
        }

        public int X
        {
            get
            {
                return m_X;
            }

            set
            {
                m_X = value;
            }
        }

        public int Y
        {
            get
            {
                return m_Y;
            }

            set
            {
                m_Y = value;
            }
        }
    }
}