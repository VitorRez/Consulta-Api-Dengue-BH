import { formatDate } from "../utils/formatDate";

const nivelBorderClasses = {
  1: "nivel-1-border",
  2: "nivel-2-border",
  3: "nivel-3-border",
  4: "nivel-4-border",
};

const nivelTextClasses = {
  1: "nivel-1-text",
  2: "nivel-2-text",
  3: "nivel-3-text",
  4: "nivel-4-text",
};

const nivelNomes = { 1: "Verde", 2: "Amarelo", 3: "Laranja", 4: "Vermelho" };

export function CardSemana({ data }) {
  if (!data) {
    return (
      <div className="card-vazio">
        <p className="text-gray-500">Sem dados</p>
      </div>
    );
  }

  const borderClass = nivelBorderClasses[data.nivel_alerta] ?? "nivel-0-border";
  const textClass = nivelTextClasses[data.nivel_alerta] ?? "nivel-0-text";

  return (
        <div className={`card ${borderClass}`}>
        <h3 className="card-titulo">{data.semana_epidemiologica}</h3>
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