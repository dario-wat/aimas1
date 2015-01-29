public interface IPathListener {

	// Method is called when the first element in the path is removed
	void UpdateReachedIndex(int idx);

	// Method is called when PathGenerator object is resetted
	void UpdateResetted();
}