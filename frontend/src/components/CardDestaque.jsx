import { formatDate } from "../utils/formatDate";

const nivelCores = { 1: "#4caf50", 2: "#ffc107", 3: "#ff9800", 4: "#f44336" };
const nivelNomes = { 1: "Verde", 2: "Amarelo", 3: "Laranja", 4: "Vermelho" };

export function CardDestaque({ titulo, data }) {
  if (!data) return null;

  const nivel = data.nivel_alerta;
  const borderClass = nivel >= 1 && nivel <= 4 ? `nivel-${nivel}-border` : "nivel-0-border";
  const textClass = nivel >= 1 && nivel <= 4 ? `nivel-${nivel}-text` : "nivel-0-text";

  return (
      <div className={`card ${borderClass}`}>
      <h3 className={`card-destaque-titulo ${textClass}`}>{titulo}</h3>
      <p className="card-destaque-semana">{data.semana_epidemiologica}</p>
      <p className="card-subtitulo">Início: {formatDate(data.data_inicio_semana)}</p>

      <p className="card-linha-espaco">
        <strong>Casos estimados:</strong> {data.casos_est}
      </p>
      <p className="card-linha">
        <strong>Casos notificados:</strong> {data.casos_notificados}
      </p>
      <p className="card-linha-final">
        <strong>Nível:</strong>{" "}
        <span className={`card-nivel ${textClass}`}>
          {data.nivel_alerta} ({nivelNomes[data.nivel_alerta] ?? "?"})
        </span>
      </p>
    </div>
  );
}