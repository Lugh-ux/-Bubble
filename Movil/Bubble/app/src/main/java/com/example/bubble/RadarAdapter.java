package com.example.bubble;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;
import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;
import java.util.List;

public class RadarAdapter extends RecyclerView.Adapter<RadarAdapter.RadarViewHolder> {

    private List<BurbujaRadar> listaRadar;

    public RadarAdapter(List<BurbujaRadar> listaRadar) {
        this.listaRadar = listaRadar;
    }

    @NonNull
    @Override
    public RadarViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_radar, parent, false);
        return new RadarViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull RadarViewHolder holder, int position) {
        BurbujaRadar burbuja = listaRadar.get(position);
        holder.txtMensaje.setText(burbuja.getMensaje());
        holder.txtDistancia.setText(burbuja.getDistancia());
    }

    @Override
    public int getItemCount() {
        return listaRadar.size();
    }

    public static class RadarViewHolder extends RecyclerView.ViewHolder {
        TextView txtMensaje, txtDistancia;

        public RadarViewHolder(@NonNull View itemView) {
            super(itemView);
            txtMensaje = itemView.findViewById(R.id.txtMensajeRadar);
            txtDistancia = itemView.findViewById(R.id.txtDistanciaRadar);
        }
    }
}