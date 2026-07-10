//---------------------------------------------------------------------------------------------------------------
// Skilful. Tactica Adversa graphic and analysis tool.
// Copyright (C) 2010 "...", Alex Kokomov(Loylick)
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
//
// mail: support@protoforma.com 
//---------------------------------------------------------------------------------------------------------------

//
// Mesh.cs: implementation of the Mesh class.
//

/// <summary>
/// Класс Mesh описывает экстремумы тренда на одном фрейме
/// </summary>


public class Mesh
{
	public int k;// номер соответсвующего экстремуму бара в массиве баров List<Bar> map.
	public decimal before; // значение предыдущего экстремума на этом фрейме
	public decimal after;  // значение последующего экстремума на этом фрейме
    public int b_be; //номер предыдущего экстремума на этом фрейме в массиве баров List<Bar> map
    public int b_af; //номер последующего экстремума на этом фрейме в массиве баров List<Bar> map

}

