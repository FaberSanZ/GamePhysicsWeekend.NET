using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulkanRenderer.Physics;

namespace VulkanRenderer
{
    public class Scene : IDisposable
    {

        public Scene() 
        { 
            m_bodies = new(128); 
        }

        public List<Body> m_bodies;


        internal void Reset()
        {
            for (int i = 0; i < m_bodies.Count; i++)
                m_bodies[i] = null;
            m_bodies.Clear();

            Initialize();
        }
        internal void Initialize()
        {
            Body body = new Body();
            body.m_position = new(0, 0, 0);
            //body.m_orientation = Quat(0, 0, 0, 1);
            //body.m_shape = new ShapeSphere(1.0f);
            m_bodies.Add(body);

            body.m_position = new(0, 0, -101);
            //body.m_orientation = Quat(0, 0, 0, 1);
            //body.m_shape = new ShapeSphere(100.0f);
            m_bodies.Add(body);

        }
        internal void Update(float dt_sec)
        {

        }


        public void Dispose()
        {
            for (int i = 0; i < m_bodies.Count; i++)
                 m_bodies[i] = null;
            m_bodies.Clear();
        }
    }
}
