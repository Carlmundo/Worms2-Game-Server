namespace fk
{
	template <class T>
	void Patch::write(const T& value)
	{
		memcpy_s(_lpAddress + dwPosition, sizeof(T), &value, sizeof(T));
		dwPosition += sizeof(T);
	}

}
