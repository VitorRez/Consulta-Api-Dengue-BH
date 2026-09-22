import {formatDate} from "../utils/formatDate";

export function TabelaDengue({ data }) {
  return (
    <table className="tabela">
      <thead className="tabela-head">
        <tr>
          <th className="tabela-th">Semana</th>
          <th className="tabela-th">Início da Semana</th>
          <th className="tabela-th">Casos Estimados</th>
          <th className="tabela-th">Casos Notificados</th>
          <th className="tabela-th">Nível</th>
        </tr>
      </thead>
      <tbody className="tabela-body">
        {data.map((d, i) => (
          <tr key={i} className="tabela-linha">
            <td className="tabela-td-bold">{d?.semana_epidemiologica ?? "-"}</td>
            <td className="tabela-td">{formatDate(d?.data_inicio_semana)}</td>
            <td className="tabela-td">{d?.casos_est ?? "-"}</td>
            <td className="tabela-td">{d?.casos_notificados ?? "-"}</td>
            <td className="tabela-td">{d?.nivel_alerta ?? "-"}</td>
          </tr>
        ))}
      </tbody>
    </table>
  );
}