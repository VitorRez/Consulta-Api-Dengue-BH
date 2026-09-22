import { useEffect, useState } from "react";
import { getExtremos } from "./api/dengueApi";
import { CardSemana } from "./components/CardSemana";
import { CardDestaque } from "./components/CardDestaque";
import { TabelaDengue } from "./components/TabelaDengue";
import { GraficoDengue } from "./components/GraficoDengue";
import { getLastWeeksWithData } from "./utils/epiWeek";

function App(){
  const [data, setData] = useState([]);
  const [extremos, setExtremos] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    async function fetchData(){
      try{
        const [semanas, ext] = await Promise.all([getLastWeeksWithData(3, 12), getExtremos(),]);
        setData(semanas);
        setExtremos(ext);
      }catch(err){
        setError("Erro ao carregar dados.");
        console.error(err);
      }finally{
        setLoading(false);
      }
    }
    fetchData();
  }, []);

  return(
    <div className="app-container">
      <div className="app-inner">
        <header className="app-header">
          <h1 className="app-titulo">Dengue em Belo Horizonte</h1>
          <p className="app-subtitulo">Últimas três semanas epidemiológicas</p>
        </header>
        {loading && <p className="app-loading">Carregando...</p>}
        {error && <p className="app-erro">{error}</p>}

        {!loading && !error &&(
          <>
            <section className="app-secao">
              <h2 className="app-secao-titulo-centro">Semanas recentes</h2>
              <div className="app-linha-cards">
                {data.map((d, i)=>(
                  <CardSemana key={i} data={d}></CardSemana>
                ))}
              </div>
            </section>
            {extremos &&(
              <section className="app-secao">
                <h2 className="app-secao-titulo-centro">
                  Destaques (todo histórico)
                </h2>
                <div className="app-linha-cards">
                  <CardDestaque titulo="Maior n° de Casos" data={extremos.maior_nivel}></CardDestaque>
                  <CardDestaque titulo="Menor n° de Casos" data={extremos.menor_nivel}></CardDestaque>
                </div>
              </section>
            )}
            <section className="app-secao">
              <h2 className="app-secao-titulo">Evolução</h2>
              <div className="app-painel">
                <GraficoDengue data={data} />
              </div>
            </section>
            <section>
              <h2 className="app-secao-titulo">Detalhes</h2>
              <div className="app-painel-tabela">
                <TabelaDengue data={data}></TabelaDengue>
              </div>
            </section>
          </>
        )}
      </div>
    </div>
  );
}

export default App;