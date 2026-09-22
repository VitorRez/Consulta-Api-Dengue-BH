import axios from "axios";

const api = axios.create({
    baseURL: import.meta.env.VITE_API_URL,
});

export async function getWeek(ew, ey){
    try{
        const {data} = await api.get("/api/dengue", {
            params: {ew, ey},
        });
        return data;
    } catch (err) {
        if(axios.isAxiosError(err) && err.response?.status === 404){
            return null;
        }
        throw err;
    }
}

export async function getExtremos() {
  try {
    const { data } = await api.get("/api/dengue/extremos");
    return data;
  } catch (err) {
    if (axios.isAxiosError(err) && err.response?.status === 404) {
      return null;
    }
    throw err;
  }
}