using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnOpenTK.Detour
{
    public class DetourNavMeshcontroller
    {
        Scene scene;
        Detour.dtNavMesh navMesh;
		List<Vertex> vertices;
		List<int> indices;
		Scene.Node node;

		public Detour.dtNavMesh NavMesh { get { return navMesh;  } }
        
        public DetourNavMeshcontroller(Scene scene)
        {
            this.scene = scene;
        }

        public bool Load(string path)
        {
            navMesh = DetourNavMeshLoader.Load(path);

            if ( navMesh != null )
            {
                PrepareRender();
            }

            return navMesh != null;
        }

		private void PrepareRender()
		{
			// Add to scene after building a mesh

			vertices = new List<Vertex>();
			indices = new List<int>();

			for (int i = 0; i < navMesh.getMaxTiles(); ++i)
			{
				Detour.dtMeshTile tile = navMesh.getTile(i);

				if (tile.header != null)
				{
					AddMeshTile(tile);
				}
			}

			var geom = new Mesh();
			geom.Load(vertices, indices, indices.Count / 3);

			var mat = new MaterialDiffuse() { ShaderProgram = "diffuse", Tex = "" };

			node = new Scene.Node() { Name = "NaviMesh", Material = mat, Mesh = geom };
			scene.Add(node);
		}

		private void AddMeshTile(Detour.dtMeshTile tile)
		{ 
			var basePoly = navMesh.getPolyRefBase(tile);
			uint tileNum = navMesh.decodePolyIdTile(basePoly);

			for (int i = 0; i < tile.header.polyCount; ++i)
			{
				var p = tile.polys[i];

				if (p.getType() == (byte)Detour.dtPolyTypes.DT_POLYTYPE_OFFMESH_CONNECTION) // Skip off-mesh links.
					continue;

				var pd = tile.detailMeshes[i];

				for (int j = 0; j < pd.triCount; ++j)
				{
					var triIndex = (pd.triBase + j) * 4;

					for (int k = 0; k < 3; ++k)
					{
						if (tile.detailTris[triIndex+k] < p.vertCount)
						{
							var detailIndex = tile.detailTris[triIndex + k];
							var vertIndex = p.verts[detailIndex] * 3;
							var vertex = new Vertex()
							{
								Position = new OpenTK.Vector3(tile.verts[vertIndex + 0], tile.verts[vertIndex + 2], tile.verts[vertIndex + 1]),
								Normal = new OpenTK.Vector3(),
								Color = new OpenTK.Vector4(0, 0, 1, 1)
							};
							vertices.Add(vertex);
							indices.Add(vertices.Count-1);
						}
						else
						{
							var detailIndex = tile.detailTris[triIndex + k];
							var vertIndex = (pd.vertBase + detailIndex - p.vertCount) * 3;
							var vertex = new Vertex()
							{
								Position = new OpenTK.Vector3(tile.detailVerts[vertIndex + 0], tile.detailVerts[vertIndex + 2], tile.detailVerts[vertIndex + 1]),
								Normal = new OpenTK.Vector3(),
								Color = new OpenTK.Vector4(0, 0, 1, 1)
							};
							vertices.Add(vertex);
							indices.Add(vertices.Count-1);
						}
					}
				}
			}
		}
    }
}
