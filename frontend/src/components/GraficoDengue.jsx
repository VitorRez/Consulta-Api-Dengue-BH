import {BarChart, Bar, XAxis, YAxis, Tooltip, Legend, CartesianGrid, ResponsiveContainer} from "recharts";

export function GraficoDengue({data}){
    const chartData = data.filter((d) => d !== null).map((d) => ({
        semana: d.semana_epidemiologica,
        "Casos Estimados": d.casos_est,
        "Casos Notificados": d.casos_notificados,
    }));

    if (chartData.length === 0) {
        return <p className="text-gray-400">Sem dados para exibir no gráfico.</p>;
    }

    return (
        <div className="w-full h-[400px]">
        <ResponsiveContainer>
            <BarChart data={chartData}>
            <CartesianGrid strokeDasharray="3 3" />
            <XAxis dataKey="semana" />
            <YAxis />
            <Tooltip
                contentStyle={{
                backgroundColor: "#1f2937",
                border: "1px solid #374151",
                color: "#f3f4f6",
                }}
                labelStyle={{ color: "#f3f4f6" }}
            />
            <Legend />
            <Bar dataKey="Casos Estimados" fill="#2196f3" />
            <Bar dataKey="Casos Notificados" fill="#ff5722" />
            </BarChart>
        </ResponsiveContainer>
        </div>
    );
}