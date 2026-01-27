package com.example.bubble;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.TextView;
import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;
import java.util.List;

public class ActividadAdapter extends RecyclerView.Adapter<ActividadAdapter.ViewHolder> {

    private List<Actividad> lista;

    public ActividadAdapter(List<Actividad> lista) {
        this.lista = lista;
    }

    @NonNull
    @Override
    public ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_feed, parent, false);
        return new ViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull ViewHolder holder, int position) {
        Actividad act = lista.get(position);
        holder.txtUsuario.setText(act.getNombreUsuario());
        holder.txtAccion.setText(act.getAccion());
        holder.txtDistancia.setText(act.getDistancia());
        // holder.imgPerfil.setImageResource(act.getImagenPerfil()); // Si tienes las imágenes
    }

    @Override
    public int getItemCount() {
        return lista.size();
    }

    public static class ViewHolder extends RecyclerView.ViewHolder {
        TextView txtUsuario, txtAccion, txtDistancia;
        ImageView imgPerfil;

        public ViewHolder(@NonNull View itemView) {
            super(itemView);
            txtUsuario = itemView.findViewById(R.id.txtUsuario);
            txtAccion = itemView.findViewById(R.id.txtAccion);
            txtDistancia = itemView.findViewById(R.id.txtDistancia);
            imgPerfil = itemView.findViewById(R.id.imgPerfil);
        }
    }
}