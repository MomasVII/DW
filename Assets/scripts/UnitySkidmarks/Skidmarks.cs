using UnityEngine;
using UnityEngine.Rendering;
using TMPro;
using UnityEngine.SceneManagement;

// Skidmarks controller. Put one of these in a scene somewhere. Call AddSkidMark.
// Copyright 2017 Nition, BSD licence (see LICENCE file). http://nition.co
public class Skidmarks : MonoBehaviour {
	// INSPECTOR SETTINGS

	[SerializeField]
	Material skidmarksMaterial; // Material for the skidmarks to use

	// END INSPECTOR SETTINGS

	const int MAX_MARKS = 600; // Max number of marks total for everyone together
	const float MARK_WIDTH = 0.35f; // Width of the skidmarks. Should match the width of the wheels
	const float GROUND_OFFSET = 0.02f;  // Distance above surface in metres
	const float MIN_DISTANCE = 0.75f; // Distance between skid texture sections in metres. Bigger = better performance, less smooth
	const float MIN_SQR_DISTANCE = MIN_DISTANCE * MIN_DISTANCE;

	private DriftScoreManager driftScoreManager;

	// Info for each mark created. Needed to generate the correct mesh
	class MarkSection {
		public Vector3 Pos = Vector3.zero;
		public Vector3 Normal = Vector3.zero;
		public Vector4 Tangent = Vector4.zero;
		public Vector3 Posl = Vector3.zero;
		public Vector3 Posr = Vector3.zero;
		public byte Intensity;
		public int LastIndex;
	};

	int markIndex;
	MarkSection[] skidmarks;
	Mesh marksMesh;
	MeshRenderer mr;
	MeshFilter mf;

	Vector3[] vertices;
	Vector3[] normals;
	Vector4[] tangents;
	Color32[] colors;
	Vector2[] uvs;
	int[] triangles;

	ParticleSystem particle_rl;
	ParticleSystem particle_rr;
	/*ParticleSystem particle_fl;
	ParticleSystem particle_fr;*/

	bool meshUpdated;
	bool haveSetBounds;

	//Play Skid Audio
	public AudioSource skidSource;

	Scene scene;

	private float sfxv;

	// #### UNITY INTERNAL METHODS ####

	protected void Start() {

		sfxv = PlayerPrefs.GetFloat("sfxVolume");

		scene = SceneManager.GetActiveScene();

		skidSource.volume = 0.0f;
		driftScoreManager = GameObject.FindObjectOfType<DriftScoreManager>();

		// Generate a fixed array of skidmarks
		skidmarks = new MarkSection[MAX_MARKS];
		for (int i = 0; i < MAX_MARKS; i++) {
			skidmarks[i] = new MarkSection();
		}

		mf = GetComponent<MeshFilter>();
		mr = GetComponent<MeshRenderer>();
		if (mr == null) {
			mr = gameObject.AddComponent<MeshRenderer>();
		}
		marksMesh = new Mesh();
		marksMesh.MarkDynamic();
		if (mf == null) {
			mf = gameObject.AddComponent<MeshFilter>();
		}
		mf.sharedMesh = marksMesh;

		vertices = new Vector3[MAX_MARKS * 4];
		normals = new Vector3[MAX_MARKS * 4];
		tangents = new Vector4[MAX_MARKS * 4];
		colors = new Color32[MAX_MARKS * 4];
		uvs = new Vector2[MAX_MARKS * 4];
		triangles = new int[MAX_MARKS * 6];

		mr.shadowCastingMode = ShadowCastingMode.Off;
		mr.receiveShadows = false;
		mr.material = skidmarksMaterial;
		mr.lightProbeUsage = LightProbeUsage.Off;

		GameObject theDemo = GameObject.FindWithTag("Player");
		particle_rl = theDemo.transform.Find("Dust").transform.Find("Particle_RL").GetComponent<ParticleSystem>();
		particle_rr = theDemo.transform.Find("Dust").transform.Find("Particle_RR").GetComponent<ParticleSystem>();


	}

	protected void LateUpdate() {


		if (!meshUpdated) return;
		meshUpdated = false;

		// Reassign the mesh if it's changed this frame
		marksMesh.vertices = vertices;
		marksMesh.normals = normals;
		marksMesh.tangents = tangents;
		marksMesh.triangles = triangles;
		marksMesh.colors32 = colors;
		marksMesh.uv = uvs;

		if (!haveSetBounds) {
			// Could use RecalculateBounds here each frame instead, but it uses about 0.1-0.2ms each time
			// Save time by just making the mesh bounds huge, so the skidmarks will always draw
			// Not sure why I only need to do this once, yet can't do it in Start (it resets to zero)
			marksMesh.bounds = new Bounds(new Vector3(0, 0, 0), new Vector3(10000, 10000, 10000));
			haveSetBounds = true;
		}

		mf.sharedMesh = marksMesh;
	}

	// #### PUBLIC METHODS ####

	// Function called by the wheel that's skidding. Sets the intensity of the skidmark section
	// by setting the alpha of the vertex color
	public int AddSkidMark(Vector3 pos, Vector3 normal, float intensity, int lastIndex) {

		if (intensity > 1) intensity = 1f;
		else if (intensity < 0) return -1;

		if (lastIndex > 0) {
			float sqrDistance = (pos - skidmarks[lastIndex].Pos).sqrMagnitude;
			if (sqrDistance < MIN_SQR_DISTANCE) return lastIndex;
		}

		MarkSection curSection = skidmarks[markIndex];

		curSection.Pos = pos + normal * GROUND_OFFSET;
		curSection.Normal = normal;
		curSection.Intensity = (byte)(intensity * 200f);
		curSection.LastIndex = lastIndex;

		if (lastIndex != -1) {
			MarkSection lastSection = skidmarks[lastIndex];
			Vector3 dir = (curSection.Pos - lastSection.Pos);
			Vector3 xDir = Vector3.Cross(dir, normal).normalized;

			curSection.Posl = curSection.Pos + xDir * MARK_WIDTH * 0.5f;
			curSection.Posr = curSection.Pos - xDir * MARK_WIDTH * 0.5f;
			curSection.Tangent = new Vector4(xDir.x, xDir.y, xDir.z, 1);

			if (lastSection.LastIndex == -1) {
				lastSection.Tangent = curSection.Tangent;
				lastSection.Posl = curSection.Pos + xDir * MARK_WIDTH * 0.5f;
				lastSection.Posr = curSection.Pos - xDir * MARK_WIDTH * 0.5f;
			}
		}

		UpdateSkidmarksMesh();
		if(normal.y == 1) { PlaySkidSound(intensity); }
		else { PlaySkidSound(0); }

		int curIndex = markIndex;
		// Update circular index
		markIndex = ++markIndex % MAX_MARKS;

		return curIndex;
	}

	public float carsSpeed;
	void PlaySkidSound(float intensity) {
		if(carsSpeed < 10) {
			skidSource.volume = 0;
		} else if(carsSpeed < 45) {
			skidSource.volume = getPercentageOf((carsSpeed-10)/100, 0, 1)*sfxv;
		} else {
			skidSource.volume = getPercentageOf(intensity*0.7f, 0, 1)*sfxv;
		}
	}

	private float getPercentageOf(float input, int min, int max) {
		return ((input - min) * 1) / (max - min);
	}

	public void checkCarSpeed(float carSpeed){
		carsSpeed = carSpeed;
	}

	// #### PROTECTED/PRIVATE METHODS ####

	// Update part of the mesh for the current markIndex
	void UpdateSkidmarksMesh() {
		MarkSection curr = skidmarks[markIndex];


		// Nothing to connect to yet
		if (curr.LastIndex == -1) return;

		MarkSection last = skidmarks[curr.LastIndex];
		vertices[markIndex * 4 + 0] = last.Posl;
		vertices[markIndex * 4 + 1] = last.Posr;
		vertices[markIndex * 4 + 2] = curr.Posl;
		vertices[markIndex * 4 + 3] = curr.Posr;

		normals[markIndex * 4 + 0] = last.Normal;
		normals[markIndex * 4 + 1] = last.Normal;
		normals[markIndex * 4 + 2] = curr.Normal;
		normals[markIndex * 4 + 3] = curr.Normal;

		tangents[markIndex * 4 + 0] = last.Tangent;
		tangents[markIndex * 4 + 1] = last.Tangent;
		tangents[markIndex * 4 + 2] = curr.Tangent;
		tangents[markIndex * 4 + 3] = curr.Tangent;

		colors[markIndex * 4 + 0] = new Color32(0, 0, 0, last.Intensity);
		colors[markIndex * 4 + 1] = new Color32(0, 0, 0, last.Intensity);
		colors[markIndex * 4 + 2] = new Color32(0, 0, 0, curr.Intensity);
		colors[markIndex * 4 + 3] = new Color32(0, 0, 0, curr.Intensity);

		uvs[markIndex * 4 + 0] = new Vector2(0, 0);
		uvs[markIndex * 4 + 1] = new Vector2(1, 0);
		uvs[markIndex * 4 + 2] = new Vector2(0, 1);
		uvs[markIndex * 4 + 3] = new Vector2(1, 1);

		triangles[markIndex * 6 + 0] = markIndex * 4 + 0;
		triangles[markIndex * 6 + 2] = markIndex * 4 + 1;
		triangles[markIndex * 6 + 1] = markIndex * 4 + 2;

		triangles[markIndex * 6 + 3] = markIndex * 4 + 2;
		triangles[markIndex * 6 + 5] = markIndex * 4 + 1;
		triangles[markIndex * 6 + 4] = markIndex * 4 + 3;

		meshUpdated = true;

		if(scene.name != "Garage") {

			if(last.Intensity > 90) {
				if (!particle_rl.isPlaying) {
					particle_rl.Play();
					particle_rr.Play();
				}
				/*particle_fl.Play();
				particle_fr.Play();*/
			} else {
				if (particle_rl.isPlaying) {
					particle_rl.Stop();
					particle_rr.Stop();
				}
				/*particle_fl.Stop();
				particle_fr.Stop();*/
			}
		}

		driftScoreManager.updateScore(last.Intensity);

	}

}
